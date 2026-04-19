# Current State (2026-04-19)

This document is a durable handover snapshot for Copilot and contributors.

## Canonical Processing Model
- Transport input type: `RawEnvelope`.
- Processing chain: `RawEnvelope` -> materialization pipeline -> `TypedEnvelope<TPayload>` -> handling pipeline.
- `ResolvingExampleEvent` is compatibility-only and built on top of `RawEnvelope`.

## Payload Source Resolution
- Payload source is resolved before payload factory execution.
- Supported source modes:
  - Inline payload from `RawEnvelope.Payload`.
  - Referenced payload via `RawEnvelope.PayloadReference` and storage lookup.
- Extension points:
  - `IPayloadSourceResolver`
  - `IPayloadStorageProvider`

## Handling Pipeline Contract
- Pipeline order is explicit and stable:
  1. `IInfrastructureStep<TPayload>` (framework/technical concerns)
  2. `IBusinessStep<TPayload>` (application/domain policy)
  3. `IEventConsumer<TPayload>` (final business endpoint)

## Testing Baseline
- Active test layers:
  - Unit
  - Contract
  - Integration
  - E2E
  - Compatibility
  - Performance
- Validation command:
  - `dotnet build ArchiScrapper.sln && dotnet test ArchiScrapper.sln --no-build`

## Continuity Rules
- Any architecture or behavior change must update:
  - `docs/decisions.md`
  - `docs/changelog.md`
  - this file (`docs/current-state.md`) when baseline changes.
