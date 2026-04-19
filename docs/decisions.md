# Decisions

## Decision Log Policy
Record significant architectural and workflow decisions in chronological order.

## Template
### DEC-YYYYMMDD-ShortTitle
- Status: Proposed | Accepted | Superseded
- Context:
- Decision:
- Alternatives considered:
- Consequences:

## Initial Decisions
### DEC-20260417-FoundationFirst
- Status: Accepted
- Context: Repository is being prepared for long-term .NET 8 library development.
- Decision: Build foundation assets before implementing domain or messaging logic.
- Alternatives considered: Start coding feature slices immediately.
- Consequences: Lower short-term delivery speed, higher long-term consistency and maintainability.

### DEC-20260419-SimplifiedEnvelopeModel
- Status: Accepted
- Context: The initial architecture concept was richer than needed and increased complexity for early delivery.
- Decision: Keep envelope model minimal and explicit. Base metadata remains focused (`FirstName`, `LastName`, `City`), without reintroducing `IEvent`.
- Alternatives considered: Keep broader event classification model with additional abstractions.
- Consequences: Faster onboarding and lower complexity, with deliberate tradeoff of fewer built-in semantics.

### DEC-20260419-RawEnvelopeAsCanonicalInput
- Status: Accepted
- Context: Processing flow required a clear transport-level input contract independent from example/specialized event types.
- Decision: Use `RawEnvelope` as canonical input for materialization and end-to-end processing; keep `ResolvingExampleEvent` as compatibility type only.
- Alternatives considered: Continue using `ResolvingExampleEvent` as primary flow input.
- Consequences: Clear boundary between transport model and compatibility/demo types, simpler extension path.

### DEC-20260419-PayloadSourceResolutionBoundary
- Status: Accepted
- Context: Payload may be delivered inline or by reference and should be resolved before payload materialization.
- Decision: Introduce `IPayloadSourceResolver` and `IPayloadStorageProvider` abstractions; materialization pipeline resolves payload via resolver.
- Alternatives considered: Resolve payload directly inside stages without dedicated abstractions.
- Consequences: Better SRP and testability; easier replacement of storage backends and resolution strategies.

### DEC-20260419-HandlingPipelineErrorPolicy
- Status: Accepted
- Context: Handling pipeline steps need a single configurable place for failure handling instead of scattered `try/catch` blocks.
- Decision: Introduce a central `IHandlingPipelineErrorHandler<TPayload>` that can resolve failures as `Retry`, `Continue`, or `Stop`; the default behavior remains stop-and-rethrow.
- Alternatives considered: Per-step ad hoc exception handling, silent swallowing, or hard-coded retries.
- Consequences: Consumers can program retry/skip/abort behavior declaratively while the framework keeps one consistent error boundary.

### DEC-20260419-PublicationErrorPolicy
- Status: Accepted
- Context: Publish-side composition also needs a single programmable place to decide whether to retry, continue with fallback output, or abort when serialization or storage fails.
- Decision: Add `IEnvelopePublicationErrorHandler<TPayload>` and make `IEnvelopePublicationPipeline` / `IEnvelopePublisher<TPayload>` accept an optional handler per call; default behavior is stop-and-rethrow.
- Alternatives considered: Separate ad hoc `try/catch` blocks inside publisher methods, or global static policy.
- Consequences: Publication can be customized at the call site without introducing a broader orchestration framework.
