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

### DEC-20260419-PublicationPolicyBuilderAndTelemetry
- Status: Accepted
- Context: Call-site handlers are useful, but users also need a reusable way to compose publish policy and observe publication steps.
- Decision: Add `IEnvelopePublicationPolicyBuilder<TPayload>` and `IEnvelopePublicationTelemetry<TPayload>`; publish APIs gain policy-based overloads alongside the existing handler-based overloads.
- Alternatives considered: Hard-code telemetry into the pipeline, or require users to wire separate handler and telemetry objects manually.
- Consequences: Publication policy can be built once, reused across calls, and instrumented without changing the core pipeline.

### DEC-20260419-DefaultPublicationPolicyProfile
- Status: Accepted
- Context: Users need a low-friction default profile that avoids manual composition of retry/fallback behavior for common publish failures.
- Decision: Add `UseDefaultProfile(...)` extension for `IEnvelopePublicationPolicyBuilder<TPayload>` backed by `DefaultEnvelopePublicationErrorHandler<TPayload>`.
- Alternatives considered: Require custom error handler implementation for every service.
- Consequences: Faster onboarding and consistent default error behavior, while preserving full customization through explicit handlers.

### DEC-20260419-UserTestingReleaseBaseline
- Status: Accepted
- Context: The repository is ready for external user testing and needs a stable release marker with minimal operational risk.
- Decision: Set a `1.0.0` version baseline in shared build properties and prioritize minimal unblockers that keep the current architecture unchanged.
- Alternatives considered: Delay user testing until broader roadmap completion, or introduce larger refactors before first release.
- Consequences: Users can start validation now on a stable baseline, while deferred enhancements continue in future iterations.
