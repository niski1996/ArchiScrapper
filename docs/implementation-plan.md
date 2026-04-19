# Implementation Plan: Event-Driven Common Library

> Status note (2026-04-19): This document contains initial architecture concepts. Current accepted baseline and implementation direction are defined in `docs/decisions.md`, `docs/changelog.md`, and `.github/copilot-instructions.md`.

## 1. Architecture Overview

### 1.1 Role and Layering

This library is a **domain-specific facade** over a lower-level messaging infrastructure (Layer 1).

**Layer 1 (underlying)**: generic messaging transport, broker integration, basic publish/subscribe.  
**Layer 2 (this library)**: domain event abstractions, payload handling, processing pipeline, convenience API for application teams.

**Design principle**: Delegate infrastructure concerns to Layer 1; own domain-specific abstractions, conventions, and developer experience.

### 1.2 Core Problem Statement

Application teams need:
- Unified way to publish and handle events
- Transparent handling of payloads (inline vs reference-based)
- Optional payload compression, encryption, and decryption
- Modular, composable processing pipeline
- No implicit magic; clear, explicit flow

### 1.3 Key Constraints

- Payloads range from bytes to several megabytes (not suitable for always-inline storage).
- Payload may be compressed and/or encrypted before transport.
- Payload may be stored externally and referenced by ID.
- Processing pipeline must be deterministic and testable.
- Multiple transformation strategies must co-exist (gzip, brotli, encryption, etc.).

---

## 2. Core Domain Model

### 2.1 Event Classification

Events are classified by their payload strategy:

```
IEvent
├── IHasPayload
│   └── Payload is inline in event metadata/headers
├── IHasPayloadReference
│   └── Payload is external; event contains reference (ID, URL, etc.)
└── (no payload)
    └── Pure metadata event (no payload)
```

**Key insight**: Event type determines processing flow, not transport mechanics.

### 2.2 Payload Strategy Pattern

Two mutually-exclusive payload strategies:

**Inline Strategy**:
- Payload bytes stored directly in event.
- Suitable for small messages.
- Retrieved immediately; no additional I/O.

**Reference Strategy**:
- Payload stored in external system (e.g., S3, blob storage, custom service).
- Event contains reference (ID, location, metadata).
- Requires separate fetch during processing.
- Suitable for large payloads.

**Responsibility boundary**:
- Layer 1 handles storage mechanics (credentials, endpoints, protocol).
- Layer 2 models the choice and coordinates retrieval.

### 2.3 Payload Transformations

Three transformation categories:

**Compression**: Reduce size in transit (gzip, brotli, etc.).  
**Encryption**: Secure data at rest and in transit.  
**NoOp**: Pass-through (identity transformation).

**Transformations are**:
- **Composable**: Multiple transformations can be chained.
- **Reversible**: Each has an inverse (compress/decompress, encrypt/decrypt).
- **Optional**: No transformation is a valid choice.
- **Configurable** per event type or globally.

**Design principle**: Transformations are middleware steps in a pipeline, not integral to event handling.

---

## 3. Main Components

### 3.1 Envelope (Message Metadata Contract)

**Purpose**: Carries metadata required by Layer 1 and Layer 2 to process an event.

**Responsibilities**:
- Standard fields: EventId, EventType, Timestamp, CorrelationId, CausationId.
- Payload metadata: size, checksum, compressionType, encryptionType.
- Payload location: inline (marker) or reference (URL, storage ID, etc.).
- Version and schema info.
- Headers: extensible key-value pairs for custom metadata.

**Ownership**: Defined in Layer 2; built by Publisher, consumed by Handler.

**Key principle**: Envelope is immutable and auditable.

### 3.2 Payload Resolver

**Purpose**: Retrieve payload from inline storage or external reference.

**Contracts**:
- `IPayloadResolver`: Determines how to retrieve payload bytes given envelope metadata.
  - Implementations: `InlinePayloadResolver`, `ReferencePayloadResolver`.
  - Strategy pattern: resolver is selected based on envelope payload metadata.

**Responsibility**:
- Fetch payload from the appropriate source.
- Return raw bytes.
- Handle retrieval errors and timeouts.
- Do NOT apply transformations; only retrieve.

**Non-responsibility**: Encryption, compression, storage service integration (delegated to Layer 1).

### 3.3 Payload Transformation Pipeline

**Purpose**: Apply and reverse transformations (compression, encryption) on payload bytes.

**Core interfaces**:

```
IPayloadTransformation
├── Name: string (e.g., "gzip", "aes-256")
├── Apply(byte[] input) -> byte[] output
└── Reverse(byte[] input) -> byte[] output

IPayloadTransformationPipeline
├── AddStep(IPayloadTransformation)
├── ExecuteForward(byte[] input) -> byte[]
└── ExecuteReverse(byte[] input) -> byte[]
```

**Key design**:
- Pipeline is ordered; transformations execute in sequence.
- Each step is independent and testable.
- Failures in a step are reported with context (which step, input/output sizes).

**Registration**:
- Transformations registered in DI container or builder.
- Pipeline built per event type or globally.

**Non-responsibility**: Layer 1 credential/key management; Layer 2 only orchestrates transformations.

### 3.4 Event Publishing Pipeline

**Purpose**: Orchestrate all steps from raw event to published message.

**Pipeline steps (in order)**:

1. **Serialize Event**: Convert domain event to envelope + payload bytes.
2. **Resolve Payload Location**: Determine inline vs reference based on size/config.
3. **Apply Transformations**: Compress and/or encrypt payload.
4. **Store Payload (if reference)**: Write to external storage; get reference ID.
5. **Build Final Envelope**: Attach payload metadata and reference.
6. **Publish to Layer 1**: Hand off to underlying messaging system.
7. **Emit Telemetry**: Log, metrics, traces.

**Orchestrator contract**:

```
IEventPublisher
├── Publish<TEvent>(TEvent evt) -> PublishResult
├── PublishBatch<TEvent>(IEnumerable<TEvent> events) -> BatchPublishResult
└── Settings: IEventPublisherSettings
    ├── DefaultPayloadStrategy (inline / reference)
    ├── PayloadSizeThreshold
    ├── TransformationPipeline
    └── StorageService reference
```

**Responsibility hierarchy**:
- `EventPublisher`: orchestrates the overall flow.
- `PayloadStrategySelector`: decides inline vs reference.
- `PayloadTransformationPipeline`: applies transformations.
- `IPayloadStorage` (Layer 1 adapter): stores external payloads.

### 3.5 Event Processing Pipeline

**Purpose**: Orchestrate all steps from received message to handler invocation.

**Pipeline steps (in order)**:

1. **Receive Envelope**: Layer 1 passes received message as envelope.
2. **Resolve Payload**: Fetch from inline or external storage.
3. **Reverse Transformations**: Decrypt and/or decompress.
4. **Deserialize**: Convert envelope + payload bytes to domain event.
5. **Materialize Context**: Build data dictionary or typed context for handler.
6. **Dispatch to Handler**: Invoke appropriate handler.
7. **Emit Telemetry**: Log, metrics, traces.

**Orchestrator contract**:

```
IEventHandler<TEvent>
├── Execute(TEvent evt, IEventContext ctx) -> Task

IEventDispatcher
├── Register<TEvent>(IEventHandler<TEvent> handler)
├── Dispatch(Envelope envelope) -> Task
└── Settings: IEventProcessorSettings
    ├── TransformationPipeline
    ├── PayloadResolver
    └── ErrorHandler strategy
```

**Key design**:
- Handlers are generic and type-safe.
- Dispatcher maps envelope type to handler.
- Error handling is configurable (retry, dead-letter, skip).

### 3.6 Payload Storage Service

**Purpose**: Interface to external payload storage (S3, blob storage, custom).

**Contracts** (Layer 2 → Layer 1 boundary):

```
IPayloadStorage
├── Store(byte[] payload, StorageMetadata) -> StorageReference
├── Retrieve(StorageReference) -> byte[]
└── Delete(StorageReference) -> Task
```

**Responsibility**:
- Layer 1 implements concrete storage (AWS S3, Azure Blob, etc.).
- Layer 2 calls the interface, doesn't care about implementation.

**Configuration**:
- Endpoint, credentials, bucket, access policies: Layer 1 config.
- When to use storage (size threshold): Layer 2 logic.

### 3.7 Error and Retry Handling

**Purpose**: Handle failures in pipeline steps gracefully.

**Contracts**:

```
IEventProcessingError
├── ErrorType: enum (payload retrieval failed, transformation failed, handler threw, etc.)
├── Exception: original exception
├── Envelope: the message that failed
├── Context: step-specific details

IEventErrorHandler
├── Handle(IEventProcessingError error) -> ErrorResolution
    └── enum: Retry, DeadLetter, Skip, Throw
├── RetryStrategy: IRetryPolicy (exponential backoff, max attempts, etc.)
```

**Key principle**:
- Errors are explicit; no silent failures.
- Each pipeline step can fail independently.
- Retry and dead-letter decisions are configurable.

**Non-responsibility**: Layer 1 handles transport-level retries (broker guarantees); Layer 2 handles application-level retries (handler logic, data consistency).

---

## 4. Composition Strategies

### 4.1 Dependency Injection

**Primary composition mechanism**: Constructor injection via DI container.

**Services registered**:

```
services.AddEventPublisher()
  .AddPayloadTransformation("gzip", new GzipTransformation())
  .AddPayloadTransformation("aes256", new AesTransformation())
  .AddPayloadStorage(layer1StorageService)
  .ConfigurePublishingPipeline(opts => { ... })

services.AddEventDispatcher()
  .AddHandler<DomainEvent, DomainEventHandler>()
  .ConfigureProcessingPipeline(opts => { ... })
```

**Benefits**:
- Testable: mock dependencies in unit tests.
- Flexible: swap implementations without code changes.
- Clear: DI configuration explicitly shows what's used.

### 4.2 Pipeline Builder Pattern

**Purpose**: Construct publishing and processing pipelines programmatically.

**Contracts**:

```
IEventPublishingPipelineBuilder
├── WithPayloadStrategy(strategy: IPayloadStrategy)
├── WithCompression(algorithm: ICompressionAlgorithm)
├── WithEncryption(algorithm: IEncryptionAlgorithm)
├── WithStorageService(storage: IPayloadStorage)
├── WithTelemetry(telemetry: ITelemetryWriter)
└── Build() -> IEventPublishingPipeline

IEventProcessingPipelineBuilder
├── WithPayloadResolver(resolver: IPayloadResolver)
├── WithCompression(algorithm: ICompressionAlgorithm)
├── WithEncryption(algorithm: IEncryptionAlgorithm)
├── WithErrorHandler(handler: IEventErrorHandler)
└── Build() -> IEventProcessingPipeline
```

**Key principle**:
- Builder is fluent and intentional.
- No magic; every step is explicit.
- Immutable pipeline after build.

### 4.3 Strategy Pattern for Payload

**Purpose**: Encapsulate inline vs reference payload decision.

**Contract**:

```
IPayloadStrategy
├── Decide(payload: byte[], config: IPayloadStrategyConfig) -> PayloadLocation
    └── enum PayloadLocation: Inline, Reference

IPayloadStrategyConfig
├── MaxInlinePayloadSize: int
├── PreferredStorage: IPayloadStorage
├── RetentionPolicy: TimeSpan (for external storage)
```

**Implementations**:
- `AlwaysInlineStrategy`: All payloads stored inline (simple, limited scale).
- `SizeThresholdStrategy`: Inline if < N bytes; reference otherwise.
- `TypeBasedStrategy`: Rules per event type (e.g., UserCreated inline, LargeDataset reference).

**Responsibility**: Strategy is selected at publish time; decision is encoded in envelope.

### 4.4 Avoid "God Objects"

**Anti-pattern to avoid**:
- A single `EventPublisher` doing serialization, transformation, storage, Layer 1 publish, telemetry.
- A single `EventHandler` doing deserialization, transformation, context building, dispatch, error handling.

**Solution**: Each responsibility is a separate component.
- `EventPublisher` orchestrates; delegates to `PayloadTransformationPipeline`, `PayloadStorage`, `IEventSerializer`.
- `EventDispatcher` orchestrates; delegates to `PayloadResolver`, `PayloadTransformationPipeline`, `IEventDeserializer`, handler lookup.
- Single Responsibility Principle applied strictly.

---

## 5. Architectural Concerns and Solutions

### 5.1 Combinatorial Explosion of Configurations

**Problem**: Too many combinations of transformation strategies, payload strategies, storage types could lead to complex configuration and testing burden.

**Mitigation**:
- Limit transformation combinations to well-tested sets (e.g., no compression + encryption ordering issues).
- Use Strategy pattern to bind combinations to logical profiles (e.g., "HighSecurity", "HighSpeed", "Standard").
- Provide sensible defaults; override only when justified.
- Document breaking combinations.

### 5.2 Backwards Compatibility

**Problem**: Envelope format evolves; old consumers may not understand new metadata fields.

**Mitigation**:
- Version envelope schema (e.g., `EnvelopeVersion: 2`).
- Extend via headers: new producers add headers; old consumers ignore them gracefully.
- Transformation registry versioned per algorithm (e.g., "gzip_v1", "gzip_v2").
- Dead-letter unknown event types; document migration path.

**Assumption**: Layer 1 handles wire-level compatibility; Layer 2 handles Envelope contract evolution.

### 5.3 Payload Retrieval Failures

**Problem**: External storage service is down or payload was deleted; processing fails.

**Mitigation**:
- `IPayloadResolver` throws explicit exception with retrieval context (reference, storage type, attempt count).
- `IEventErrorHandler` catches, decides: retry, dead-letter, or skip.
- Telemetry captures failure details for diagnostics.
- SLA for storage service availability is part of non-functional requirements doc.

### 5.4 Transformation Ordering and Idempotence

**Problem**: Transformations applied in wrong order or applied twice cause data corruption.

**Mitigation**:
- Envelope explicitly records transformation order and types applied.
- Reverse pipeline checks order matches envelope; throws if unexpected.
- Transformation steps are idempotent by nature (compress-decompress cycle should yield original bytes).
- Unit tests verify idempotence for each transformation.

### 5.5 Performance: Lazy Payload Resolution

**Problem**: Fetching large payloads from storage is expensive; may not always be needed (e.g., correlationId extraction doesn't require full payload).

**Mitigation**:
- `IPayloadResolver` is lazy: called only when handler needs data.
- Envelope separates lightweight metadata (event type, IDs) from payload reference.
- Handler receives `ILazyPayload` interface; resolution deferred until `.Get()` called.

**Risk**: Increases complexity; only apply if profiling shows benefit.

### 5.6 Handling Mixed Event Types

**Problem**: Dispatcher receives heterogeneous envelopes for different event types; must route to correct handler.

**Mitigation**:
- Envelope includes `EventType: string` (FQN of domain event class).
- Dispatcher maintains registry: `EventType -> IEventHandler<T>`.
- Type resolution is explicit and testable.
- Unknown event type → configurable handler (log, dead-letter, skip).

### 5.7 Testing Challenges

**Problem**: Pipeline has many steps, configs, and integrations; testing complexity grows.

**Mitigation**:
- Each step independently testable with mock dependencies (Payload Resolver, Storage, Transformation).
- Use builder pattern to construct test scenarios.
- Provide test doubles for Layer 1 (fake storage, fake broker).
- Integration tests cover happy path; unit tests cover each step failure scenario.

---

## 6. Design Principles Applied

### 6.1 Single Responsibility Principle

Each component owns one reason to change:
- `EventPublisher`: publish orchestration logic changes.
- `PayloadTransformationPipeline`: transformation composition changes.
- `IPayloadResolver`: payload retrieval strategy changes.
- `IEventErrorHandler`: error handling policy changes.

### 6.2 Open/Closed Principle

Pipeline is open for extension:
- New transformations added by implementing `IPayloadTransformation`.
- New payload strategies added by implementing `IPayloadStrategy`.
- New storage backends added by implementing `IPayloadStorage`.
- No modifications to existing Publisher/Dispatcher code.

### 6.3 Liskov Substitution Principle

All transformation implementations are substitutable:
- Any `IPayloadTransformation` can replace another without breaking the pipeline.
- Any `IPayloadStorage` can replace another without breaking publisher.
- Contracts enforce substitutability; implementation details are opaque.

### 6.4 Interface Segregation Principle

Clients depend on minimal, focused interfaces:
- Handler authors use `IEventHandler<T>` and `IEventContext`.
- Pipeline builders use facade builders, not monolithic configuration object.
- Storage consumers use `IPayloadStorage`, not full storage client API.

### 6.5 Dependency Inversion Principle

High-level modules (Publisher, Dispatcher) depend on abstractions (IPayloadTransformation, IPayloadStorage), not low-level details.
- Publishers don't know about S3; they know about IPayloadStorage.
- Handlers don't know about Kafka; they know about IEventContext.
- Layer 1 bindings are injected at composition root.

---

## 7. Key Interfaces Summary

```
// Event model
IEvent
IHasPayload
IHasPayloadReference

// Envelope and payload
IEnvelope
IPayloadResolver (Inline, Reference variants)

// Transformations
IPayloadTransformation (Apply, Reverse)
IPayloadTransformationPipeline

// Publishing
IEventPublisher
IEventPublishingPipeline
IPayloadStrategy

// Processing
IEventHandler<TEvent>
IEventDispatcher
IEventProcessingPipeline
IEventContext

// Storage (Layer 1 boundary)
IPayloadStorage

// Error handling and observation
IEventErrorHandler
IEventProcessingError
ITelemetryWriter
```

---

## 8. Implementation Phases

### Phase 1: Foundation (Sprint 1-2)
- Define `IEnvelope`, `IEvent`, `IHasPayload`, `IHasPayloadReference`.
- Implement `InlinePayloadResolver`, `ReferencePayloadResolver`.
- Create `IPayloadTransformation` interface and `NoOpTransformation` (baseline).
- Build `IPayloadTransformationPipeline`.
- Create `IPayloadStorage` boundary (no implementation; Layer 1 owns it).

**Deliverable**: Core abstractions, ready for integration with Layer 1.

### Phase 2: Publishing Pipeline (Sprint 2-3)
- Implement `IEventPublisher` orchestrator.
- Implement `IPayloadStrategy` and `SizeThresholdStrategy`.
- Build `EventPublishingPipelineBuilder`.
- Integrate with Layer 1 broker publish mechanism.
- Implement telemetry hooks.

**Deliverable**: Applications can publish events with payload handling.

### Phase 3: Processing Pipeline (Sprint 3-4)
- Implement `IEventDispatcher` orchestrator.
- Implement `IEventErrorHandler` with retry/dead-letter strategies.
- Build `EventProcessingPipelineBuilder`.
- Implement generic `IEventHandler<T>` support.
- Integrate with Layer 1 message consumption.

**Deliverable**: Applications can handle events with payload resolution.

### Phase 4: Transformations (Sprint 4-5)
- Implement `GzipTransformation`, `BrotliTransformation`.
- Implement encryption transformation (AES-256 or similar).
- Build transformation registration and selection in DI.
- Performance tests for large payloads with transformations.

**Deliverable**: Applications can use compression and encryption transparently.

### Phase 5: Error Handling and Resilience (Sprint 5-6)
- Implement poison-message handler (dead-letter queue, skipping).
- Implement retry policies (exponential backoff, jitter).
- Add observability (structured logging, metrics, distributed tracing).
- Implement telemetry pipeline.

**Deliverable**: Resilient, observable system with clear error semantics.

### Phase 6: Integration and Samples (Sprint 6-7)
- Create sample applications (publisher, subscriber).
- Document usage patterns and configuration.
- Performance benchmarks and optimization.
- Update architecture docs with implementation learnings.

**Deliverable**: Production-ready library with reference implementations.

---

## 9. Non-Functional Requirements

### Performance
- Payload resolution should not block dispatcher; support async resolution.
- Transformation pipeline should be efficient; avoid unnecessary copies.
- Batch processing should be supported for high-throughput scenarios.

### Reliability
- All errors are logged with context (envelope, step, operation).
- Retries are configurable and observable.
- Dead-letter queue is always reachable (no silent failures).

### Observability
- Structured logging at each pipeline step.
- Metrics: publish/subscribe rate, latency, error count.
- Distributed tracing support (correlation IDs).

### Security
- Encryption keys are not exposed in logs or telemetry.
- Payload storage credentials are managed by Layer 1.
- Untrusted envelope data is validated before processing.

### Maintainability
- All pipeline steps are independently testable.
- Code is documented with examples and rationale.
- Configuration is explicit and auditable.

---

## 10. Open Questions for Layer 1 Clarification

1. **Delivery guarantees**: Does the broker provide at-most-once, at-least-once, or exactly-once?
2. **Envelope format**: What fields must Layer 1 broker recognize in the envelope?
3. **Serialization**: Does Layer 1 handle JSON/protobuf serialization, or does Layer 2 own it?
4. **Error propagation**: If payload retrieval fails, how should Layer 2 signal failure to Layer 1?
5. **Credentials and keys**: How are storage credentials and encryption keys injected into Layer 2?
6. **Batch operations**: Does Layer 1 support batch publish/subscribe for performance?

---

## 11. Success Criteria

- [ ] Core interfaces defined and documented.
- [ ] DI integration with .NET dependencies is clean and testable.
- [ ] Publishing pipeline works end-to-end with at least one transformation.
- [ ] Processing pipeline dispatches to typed handlers correctly.
- [ ] Error scenarios (storage down, transformation failure) are handled gracefully.
- [ ] Sample applications demonstrate all major features.
- [ ] Performance benchmarks show acceptable latency (<100ms for payload resolution, transformation, dispatch).
- [ ] 80%+ code coverage for business logic; 100% for critical paths.
- [ ] Architecture docs and inline code comments explain design decisions.
