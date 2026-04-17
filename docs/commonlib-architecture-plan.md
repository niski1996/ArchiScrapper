# CommonLib Architecture Plan

## 1. High-Level Architecture

### 1.1 Purpose
CommonLib should be a modular .NET 8 library that gives application teams a consistent way to publish and handle events with per-event pipelines, while staying thin enough to delegate infrastructure mechanics to the underlying messaging layer.

### 1.2 Architectural Role
CommonLib is a domain-facing facade, not a transport engine.

- Layer 1: internal messaging infrastructure, broker integration, transport, delivery, retry, and low-level serialization mechanics.
- Layer 2: this library, responsible for event classification, pipeline composition, payload conventions, and a fluent developer API.

The core rule is simple: if a concern is already solved by Layer 1, CommonLib should expose it or configure it, not reimplement it.

### 1.3 Main Modules

#### Abstractions
Contracts used by publishers, handlers, storage, compression, encryption, serialization, and pipeline steps.

#### Builders
Fluent configuration objects for global registration, event-specific setup, publisher setup, and handler setup.

#### Pipelines
Ordered execution models for publish and handle flows.

#### Publishers
Orchestrators that turn a domain event into a message envelope and publish it.

#### Handlers
Orchestrators that receive a message envelope, reconstruct payload state, and dispatch to a consumer handler.

#### Strategies
Swappable implementations for storage, compression, encryption, serialization, and payload resolution.

#### Configuration
Strongly typed options and event profile definitions.

#### Extensions
DI registration helpers and fluent extension methods.

### 1.4 Dependency Direction
The dependency graph should stay one-directional:

- Extensions depend on Builders and Configuration.
- Builders depend on Abstractions and Options.
- Pipelines depend on Abstractions and Strategies.
- Publishers and Handlers depend on Pipelines and Abstractions.
- Strategies depend on Abstractions only.

No layer should depend on concrete infrastructure providers directly unless they are registered through DI and resolved by abstraction.

### 1.5 System Layers

#### Layer A: Developer API
The fluent `AddMessaging(...)` entry point and per-event builder surface.

#### Layer B: Composition
Event profiles, pipeline definitions, and registration metadata.

#### Layer C: Execution
Publish and handle pipeline engines.

#### Layer D: Integration
Concrete storage, compression, encryption, and messaging infrastructure adapters.

---

## 2. How to Use Each Pattern

### 2.1 Fluent Builder Pattern

Where to use:
- Global registration.
- Event-specific configuration.
- Publisher setup.
- Handler setup.

Why it fits:
- The library is configuration-heavy and must remain readable for application teams.
- Fluent chaining keeps setup discoverable without exposing internals.

Problems solved:
- Avoids massive constructors.
- Keeps configuration close to the event type.
- Makes the API expressive for per-event differences.

Example components:
- `MessagingBuilder`
- `EventBuilder<TEvent>`
- `PublisherBuilder<TEvent>`
- `HandlerBuilder<TEvent, THandler>`

### 2.2 Strategy Pattern

Where to use:
- Compression algorithm selection.
- Encryption algorithm selection.
- Storage provider selection.
- Payload resolution strategy.

Why it fits:
- Each algorithm is a replaceable policy.
- The core pipeline should not know the details of gzip, AES, S3, or any custom provider.

Problems solved:
- Enables extension without modifying core.
- Prevents conditional branching on algorithm type.
- Allows event-specific policy selection.

Example components:
- `ICompressionStrategy`
- `IEncryptionStrategy`
- `IStorageStrategy`
- `IPayloadLocationStrategy`

### 2.3 Factory Pattern

Where to use:
- Creating pipeline instances from registered event profiles.
- Resolving the correct publisher or handler execution model.
- Selecting strategy implementations based on configuration metadata.

Why it fits:
- The runtime needs to create the right composition from declarative configuration.

Problems solved:
- Centralizes creation logic.
- Avoids leaking construction details into the API surface.
- Keeps builders focused on intent, not object graphs.

Example components:
- `IPipelineFactory`
- `IEventProfileFactory`
- `IMessageProcessorFactory`

### 2.4 Chain of Responsibility / Pipeline Pattern

Where to use:
- Publish flow.
- Handle flow.
- Optional custom steps.

Why it fits:
- Publish and handle are ordered transformations with clear step boundaries.
- Each step should be independently testable and replaceable.

Problems solved:
- Avoids monolithic publish/handle services.
- Supports event-specific step ordering.
- Makes it easy to insert custom behavior.

Example components:
- `IPublishStep`
- `IHandleStep`
- `IPipelineContext`
- `PublishPipeline`
- `HandlePipeline`

### 2.5 Decorator Pattern

Where to use:
- Cross-cutting wrappers around storage, serialization, logging, and handler execution.
- Optional instrumentation or auditing layers.

Why it fits:
- Some behaviors should wrap another implementation without changing its contract.

Problems solved:
- Adds logging, metrics, validation, caching, or tracing without inheritance chains.
- Keeps core implementations small.

Example components:
- `LoggingStorageDecorator`
- `LoggingHandlerDecorator`
- `MetricsPipelineDecorator`

### 2.6 Composite Pattern

Where to use:
- Pipeline step collections.
- Event profile composition.
- Optional layered configuration segments.

Why it fits:
- A pipeline is naturally a tree/ordered composition of steps.

Problems solved:
- Treats a sequence of steps as a single executable unit.
- Lets global defaults and event-specific overrides compose cleanly.

Example components:
- `PipelineComposite<TContext>`
- `EventProfileComposite`

### 2.7 Specification Pattern

Where to use:
- Deciding which event profile applies.
- Determining whether payload should stay inline or move to external storage.
- Validating whether a given provider can handle a request.

Why it fits:
- Configuration rules are often conditional and reusable.

Problems solved:
- Keeps decision logic isolated from execution logic.
- Makes policy checks explicit and testable.

Example components:
- `IEventSpecification`
- `IPayloadInlineSpecification`
- `IProviderCapabilitySpecification`

### 2.8 Dependency Injection

Where to use:
- All runtime dependencies.
- Strategy resolution.
- Pipeline step resolution.
- Handler instantiation.

Why it fits:
- The library must be extensible without static registries.
- DI keeps runtime composition flexible and testable.

Problems solved:
- Avoids hard-coded dependencies.
- Supports multiple providers and per-event overrides.
- Simplifies testing.

Example components:
- `IServiceCollection` extensions.
- `IServiceProvider` resolution in factories.

### 2.9 Options Pattern

Where to use:
- Global library configuration.
- Event-specific configuration.
- Provider configuration.

Why it fits:
- Options represent declarative settings better than raw builder state.

Problems solved:
- Separates configuration from execution.
- Enables validation at startup.
- Supports named or keyed event profiles cleanly.

Example components:
- `MessagingOptions`
- `EventOptions<TEvent>`
- `PublisherOptions<TEvent>`
- `HandlerOptions<TEvent>`

---

## 3. Builder Design

### 3.1 Global Builder

Role:
- Entry point for registering the messaging subsystem.
- Owns library-wide defaults.
- Registers shared services and baseline conventions.

Suggested shape:
- `AddMessaging(Action<MessagingBuilder> configure)`

Responsibilities:
- Register defaults for serialization, logging, and conventions.
- Register common abstractions and factories.
- Allow per-event builder creation.

Do not put event pipeline logic here.

### 3.2 Event-Specific Builder

Role:
- Owns configuration for one event type.
- Stores the event profile used by both publisher and handler registrations.

Suggested shape:
- `MessagingBuilder.AddEvent<TEvent>(Action<EventBuilder<TEvent>> configure)`

Responsibilities:
- Define event-level defaults.
- Declare payload model.
- Attach validation and capability rules.
- Create nested publisher and handler builders.

This builder is the anchor that prevents the global builder from becoming a God Builder.

### 3.3 Publisher Builder

Role:
- Declares the publish pipeline for one event type.

Suggested shape:
- `EventBuilder<TEvent>.AddPublisher(Action<PublisherBuilder<TEvent>> configure)`

Responsibilities:
- Choose serialization policy.
- Add compression.
- Add encryption.
- Configure storage of the payload.
- Configure publish-time logging or instrumentation.

Publisher builder should only describe outbound processing.

### 3.4 Handler Builder

Role:
- Declares the inbound pipeline for one event type and handler pair.

Suggested shape:
- `EventBuilder<TEvent>.AddHandler<THandler>(Action<HandlerBuilder<TEvent, THandler>> configure)`

Responsibilities:
- Configure payload loading.
- Configure decryption and decompression order.
- Configure deserialization and validation.
- Configure dispatch behavior.

Handler builder should only describe inbound processing.

### 3.5 Builder Rules

- Every builder should be narrow and explicit.
- Builders should only collect configuration, not execute workflows.
- Builder methods should be grouped by responsibility, not by class hierarchy convenience.
- Builder output should be immutable options or registration descriptors.

---

## 4. Pipeline Design

### 4.1 Publish Pipeline

Recommended flow:

1. Serialize domain event into payload bytes.
2. Compress payload if enabled.
3. Encrypt payload if enabled.
4. Store payload externally if the strategy says so.
5. Create message envelope with payload metadata and references.
6. Publish the final message through the underlying messaging layer.

Important rule:
- The pipeline should evaluate storage and payload shape after the transformation steps are known, so the envelope always reflects the final payload state.

Suggested step types:
- `ISerializeStep`
- `ICompressStep`
- `IEncryptStep`
- `IStorePayloadStep`
- `IPublishMessageStep`

### 4.2 Handle Pipeline

Recommended flow:

1. Receive message envelope.
2. Load payload from inline content or external storage.
3. Decrypt payload if needed.
4. Decompress payload if needed.
5. Deserialize payload into event contract.
6. Dispatch to the handler.

Suggested step types:
- `IReceiveMessageStep`
- `ILoadPayloadStep`
- `IDecryptStep`
- `IDecompressStep`
- `IDeserializeStep`
- `IDispatchStep`

### 4.3 Pipeline Rules

- Publish and handle pipelines must be symmetric where possible, but not necessarily identical.
- Step order should be explicit and deterministic.
- Steps must be idempotent where the runtime allows it.
- Each step should work against a context object, not shared mutable globals.
- Each step should emit enough metadata for diagnostics and failure analysis.

### 4.4 Pipeline Context

Use separate context types for publish and handle paths.

- `PublishContext` should contain the domain event, configuration profile, payload state, and output envelope.
- `HandleContext` should contain the received envelope, raw payload, decoded payload, and handler resolution data.

This prevents accidental coupling between inbound and outbound flows.

---

## 5. Extensibility Model

### 5.1 Storage Providers

Extension model:
- Implement `IStorageStrategy`.
- Register provider in DI.
- Expose it through the fluent API as `UseStorage<TStorage>()`.

What a new provider should provide:
- Write payload.
- Read payload.
- Resolve reference metadata.
- Report capability limits.

Recommended rule:
- Storage providers should never be coupled to event types directly; they should operate on a generic storage contract.

### 5.2 Compression Providers

Extension model:
- Implement `ICompressionStrategy`.
- Register in DI.
- Enable via `UseCompression<TCompression>()`.

Requirements:
- Forward and reverse operations.
- Deterministic output metadata.
- Clear failure modes for unsupported data.

### 5.3 Encryption Providers

Extension model:
- Implement `IEncryptionStrategy`.
- Register in DI.
- Enable via `UseEncryption<TEncryption>()` and `UseDecryption<TEncryption>()`.

Requirements:
- Encrypt and decrypt symmetrically.
- Expose algorithm identifiers for envelope metadata.
- Integrate with key material through external configuration, not hard-coded secrets.

### 5.4 Custom Steps

Extension model:
- Implement a pipeline step interface.
- Insert step through builder configuration.

Use cases:
- validation
- auditing
- transformation
- metrics
- tenant enrichment

Rules:
- Custom steps should not mutate unrelated configuration.
- Custom steps should receive only the context they need.

### 5.5 Custom Handlers

Extension model:
- Register handler type in DI.
- Bind it to an event profile through `AddHandler<TEvent, THandler>()`.

Rules:
- Handler execution should be isolated from pipeline assembly.
- Handler lifetime should be explicit and container-managed.

### 5.6 Extensibility Principles

- Add new behavior by adding a strategy, step, or decorator.
- Avoid modifying core pipeline orchestration for a new provider.
- Do not add event-specific branches inside shared infrastructure.
- Prefer registration-time composition over runtime conditionals.

---

## 6. Suggested Folder Structure (.NET)

```text
CommonLib/
  Abstractions/
    Events/
    Payloads/
    Pipelines/
    Storage/
    Compression/
    Encryption/
    Logging/
  Builders/
    MessagingBuilder.cs
    EventBuilder.cs
    PublisherBuilder.cs
    HandlerBuilder.cs
  Pipelines/
    Publish/
    Handle/
    Context/
    Steps/
  Publishers/
    EventPublisher.cs
    Factories/
  Handlers/
    EventHandlerDispatcher.cs
    Factories/
  Strategies/
    Storage/
    Compression/
    Encryption/
    Serialization/
  Decorators/
    Logging/
    Metrics/
  Extensions/
    ServiceCollectionExtensions.cs
    BuilderExtensions.cs
  Configuration/
    MessagingOptions.cs
    EventOptions.cs
    PublisherOptions.cs
    HandlerOptions.cs
  Validation/
  Diagnostics/
```

### Structure Rules

- Keep abstraction contracts separate from implementations.
- Keep pipeline steps in their own subfolders.
- Keep strategy implementations grouped by concern, not by event type.
- Keep public API surface small and intentional.

---

## 7. Risks and Anti-Patterns

### 7.1 God Builder

Risk:
- One giant builder becomes the place where all configuration and all behavior accumulate.

Avoid by:
- Splitting global, event, publisher, and handler builders.
- Ensuring builders only define configuration, not execution.

### 7.2 Too Many Generics

Risk:
- The API becomes unreadable and difficult to diagnose.

Avoid by:
- Keeping generics only at event and handler boundaries.
- Using options and descriptors for most internal composition.

### 7.3 Too Many Interfaces

Risk:
- Interface inflation makes the design feel abstract but not actually simpler.

Avoid by:
- Introducing interfaces only when there is a real alternate implementation or a test seam.
- Prefer a small set of stable contracts.

### 7.4 Runtime Spaghetti

Risk:
- Conditional logic branches explode at runtime and become impossible to reason about.

Avoid by:
- Building pipelines at registration time.
- Selecting strategies via DI and profile metadata.

### 7.5 Duplicated Pipelines

Risk:
- Publish and handle logic drift apart and duplicate shared concerns.

Avoid by:
- Reusing step contracts where symmetry exists.
- Sharing metadata and helper abstractions, not full execution logic.

### 7.6 Tight Coupling

Risk:
- Storage, encryption, and compression become bound to concrete providers and event types.

Avoid by:
- Depending on abstractions.
- Keeping provider selection behind factories and strategies.

### 7.7 Hidden Cross-Cutting Concerns

Risk:
- Logging, metrics, and diagnostics leak into business flow classes.

Avoid by:
- Using decorators or pipeline steps for cross-cutting concerns.

---

## 8. Recommended Final Approach

### 8.1 Best-Fit Architecture

The strongest option for this case is:

- Fluent Builder API for developer ergonomics.
- Options objects as the immutable output of configuration.
- Strategy Pattern for storage, compression, encryption, and payload resolution.
- Pipeline Pattern for publish and handle execution.
- Decorators for logging, metrics, and other cross-cutting concerns.
- Factory Pattern for turning configuration into executable pipelines.
- Dependency Injection as the runtime composition mechanism.
- Specification Pattern for profile selection and capability checks.

### 8.2 Why This Is the Right Balance

This approach keeps CommonLib extensible without turning it into a framework inside a framework.

- It avoids a God Builder by splitting responsibilities early.
- It avoids combinatorial explosion by composing steps and strategies instead of generating one class per permutation.
- It keeps per-event behavior explicit and testable.
- It supports new providers without modifying core orchestration.
- It keeps the public API approachable for application teams.

### 8.3 Final Recommendation

Treat the library as a composition engine for event-specific messaging profiles.

The recommended core is not a class hierarchy explosion, but a registration-time composition model:

1. The user registers an event profile.
2. The profile declares outbound and inbound pipelines.
3. Strategies are resolved by DI.
4. Factories build executable pipelines from options.
5. Publishers and handlers run those pipelines with minimal conditional logic.

This is the most maintainable design for a multi-team CommonLib because it gives each event a tailored pipeline while keeping the core small, open for extension, and resistant to architectural drift.
