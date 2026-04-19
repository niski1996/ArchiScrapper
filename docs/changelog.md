# Changelog

All notable repository-level changes are documented in this file.

## [Unreleased]
### Added
- Initial .NET 8 repository foundation.
- AI workflow instructions and reusable prompts.
- Core architecture and planning documentation set.
- Canonical transport model `RawEnvelope` for processing flow input.
- Payload source resolution abstractions: `IPayloadSourceResolver`, `IPayloadStorageProvider`.
- Core implementations: `PayloadSourceResolver`, `InMemoryPayloadStorageProvider`.
- Materialization integration with payload source resolver.
- Multi-layer test projects and coverage for unit/contract/integration/e2e/compatibility/performance.
- DI registration extensions: `AddCommonMessagingCore()` and `AddRawEventProcessingFlow<TPayload>()`.
- Minimal end-to-end usage examples in `samples/README.md` for inline and payload-reference scenarios.
- New test project: `CommonMessaging.ConsumerSimulationTests` for real consumer-like service scenarios.
- Shared fixture utilities for consumer simulation scenarios (`ConsumerSimulationTestHost<TPayload>` and `ConsumerSimulationScenarioBuilder<TPayload>`).
- Initial publish-side preparation pipeline for composing `RawEnvelope` from `TypedEnvelope<TPayload>`.
- Public publish-side facade `IEnvelopePublisher<TPayload>` / `EnvelopePublisher<TPayload>` and DI registration.
- Reference-based publish composition with payload storage write path and matching DI wiring.
- End-to-end publish -> consume roundtrip tests for inline and referenced payloads.
- Explicit inline convenience method `PublishInline(...)` on `IEnvelopePublisher<TPayload>`.
- Central handling pipeline error policy with configurable `Retry`, `Continue`, and `Stop` decisions.

### Changed
- Materialization and raw processing contracts now use `RawEnvelope`.
- `ResolvingExampleEvent` now acts as compatibility type over `RawEnvelope`.
- Tests migrated to use `RawEnvelope` in canonical processing scenarios.
- Materialization pipeline DI wiring uses explicit resolver constructor path to preserve default stage execution.
- Test strategy now includes a consumer simulation layer validating framework usability and extensibility from consumer perspective.
