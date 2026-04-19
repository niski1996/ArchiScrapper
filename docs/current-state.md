# Current State (2026-04-19)

This document is a durable handover snapshot for Copilot and contributors.

## Canonical Processing Model
- Transport input type: `RawEnvelope`.
- Processing chain: `RawEnvelope` -> materialization pipeline -> `TypedEnvelope<TPayload>` -> handling pipeline.
- `ResolvingExampleEvent` is compatibility-only and built on top of `RawEnvelope`.
- Publish-side preparation has started with `IEnvelopePublicationPipeline` and `EnvelopePublicationPipeline` for inline payload composition, and the pipeline is registered in `AddCommonMessagingCore()`.
- `IEnvelopePublisher<TPayload>` / `EnvelopePublisher<TPayload>` provide the public publish-side facade over the publication pipeline.
- `IEnvelopePublisher<TPayload>` exposes an explicit `PublishInline(...)` convenience method for the inline path.
- Publish-side reference composition is available through `ComposeWithReference(...)` / `PublishWithReference(...)` and uses the same in-memory storage boundary as consumption.
- End-to-end publish -> consume roundtrip is covered for inline and referenced payloads in E2E tests.

## Payload Source Resolution
- Payload source is resolved before payload factory execution.
- Supported source modes:
  - Inline payload from `RawEnvelope.Payload`.
  - Referenced payload via `RawEnvelope.PayloadReference` and storage lookup.
- Extension points:
  - `IPayloadSourceResolver`
  - `IPayloadStorageProvider`

## DI and Composition
- `AddCommonMessagingCore()` registers storage, payload resolver, materialization pipeline and materializer.
- `AddRawEventProcessingFlow<TPayload>()` composes handling pipeline and raw processing flow from registered steps and consumer.
- `AddEnvelopeTransportPublishingFlow<TPayload>()` composes envelope publication with transport handoff through `IRawEnvelopeTransportPublisher`.
- DI registration explicitly builds `EnvelopeMaterializationPipeline` with default internal stages to avoid empty-stage materialization.

## Publication Error Policy
- `IEnvelopePublicationPipeline` and `IEnvelopePublisher<TPayload>` accept an optional `IEnvelopePublicationErrorHandler<TPayload>` per call.
- Publication errors can be resolved as `Retry`, `Continue`, or `Stop`; default behavior is stop-and-rethrow.
- Inline publication continues with an empty payload on continue; reference publication falls back to inline output if payload storage fails and continue is chosen.

## Publication Policy Builder
- `IEnvelopePublicationPolicyBuilder<TPayload>` composes reusable publication policy from error handling and telemetry.
- `IEnvelopePublicationTelemetry<TPayload>` observes publication attempts, successes, and failures per step.
- Policy-based publish overloads are available on `IEnvelopePublicationPipeline` and `IEnvelopePublisher<TPayload>`.
- `UseDefaultProfile(...)` provides a ready-made default publication profile (retry and optional store-failure fallback) for quick adoption.

## Handling Pipeline Contract
- Pipeline order is explicit and stable:
  1. `IInfrastructureStep<TPayload>` (framework/technical concerns)
  2. `IBusinessStep<TPayload>` (application/domain policy)
  3. `IEventConsumer<TPayload>` (final business endpoint)
- Failure handling is centralized through `IHandlingPipelineErrorHandler<TPayload>` with `Retry`, `Continue`, and `Stop` decisions; default behavior is stop-and-rethrow.

## Testing Baseline
- Active test layers:
  - Unit
  - Contract
  - Integration
  - E2E
  - ConsumerSimulation
  - Compatibility
  - Performance
- Validation command:
  - `dotnet build ArchiScrapper.sln && dotnet test ArchiScrapper.sln --no-build`
- Integration tests compile with DI service-provider extensions enabled via explicit `Microsoft.Extensions.DependencyInjection` reference.
- DI registration is covered by unit test (`MessagingExtensionsRegistrationTests`).
- Consumer simulation tests use a shared `ConsumerSimulationScenarioBuilder<TPayload>` and `ConsumerSimulationTestHost<TPayload>` fixture pair.
- CI workflows are now available under `.github/workflows`:
  - PR quality gate (Unit + Contract + Integration smoke)
  - Nightly suite (Integration + E2E + ConsumerSimulation + Compatibility)
  - Release gate (Compatibility + Performance)

## Work Unit Documentation
- Work Unit 01 (`Envelope and Metadata Contract`) is documented separately in `docs/work-units/01-envelope-metadata.md`.
- Work Unit 02 (`Payload Source Resolution`) is documented separately in `docs/work-units/02-payload-source-resolution.md`.
- Work Unit 03 (`Materialization Pipeline`) is documented separately in `docs/work-units/03-materialization-pipeline.md`.
- Work Unit 04 (`Handling Pipeline and Custom Steps`) is documented separately in `docs/work-units/04-handling-pipeline.md`.
- Work Unit 05 (`DI and Composition`) is documented separately in `docs/work-units/05-di-composition.md`.

## Continuity Rules
- Any architecture or behavior change must update:
  - `docs/decisions.md`
  - `docs/changelog.md`
  - this file (`docs/current-state.md`) when baseline changes.

## Release Baseline
- Repository version baseline for user testing is `1.0.0`.
- Current focus is stabilization and feedback collection without expanding architectural scope.
