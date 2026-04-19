# Changelog

All notable repository-level changes are documented in this file.

## [Unreleased]
### Fixed
- Added missing `Microsoft.Extensions.DependencyInjection` reference in `CommonMessaging.IntegrationTests` to restore build/test compilation for integration scenarios.

### Changed
- Established repository-wide assembly/package version baseline at `1.0.0` in `Directory.Build.props` for user-testing release alignment.
- Updated root `README.md` to reflect actual implementation scope and current 1.0 user-testing workflow.

## [1.0.0] - 2026-04-19
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
- Central publication pipeline error policy with configurable `Retry`, `Continue`, and `Stop` decisions per call.
- Publication policy builder and telemetry hook for reusable publish-side configuration.
- Default publication policy profile (`UseDefaultProfile`) and sample usage for publish + telemetry.
- GitHub Actions workflows for PR quality gate, nightly suite, and release gate test execution policy.
- Transport publishing flow (`IEnvelopeTransportPublishingFlow<TPayload>`) and `IRawEnvelopeTransportPublisher` boundary for Layer 1 handoff.

### Changed
- Materialization and raw processing contracts now use `RawEnvelope`.
- `ResolvingExampleEvent` now acts as compatibility type over `RawEnvelope`.
- Tests migrated to use `RawEnvelope` in canonical processing scenarios.
- Materialization pipeline DI wiring uses explicit resolver constructor path to preserve default stage execution.
- Test strategy now includes a consumer simulation layer validating framework usability and extensibility from consumer perspective.
- Clean-code refactor reduced internal duplication in handling/publication pipelines and aligned abstraction naming (`IEventConsumer` contract file naming).
