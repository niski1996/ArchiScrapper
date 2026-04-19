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

### Changed
- Materialization and raw processing contracts now use `RawEnvelope`.
- `ResolvingExampleEvent` now acts as compatibility type over `RawEnvelope`.
- Tests migrated to use `RawEnvelope` in canonical processing scenarios.
