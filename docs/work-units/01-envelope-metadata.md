# Work Unit 01: Envelope and Metadata Contract

## Goal
Define the canonical transport envelope and its metadata contract so the framework has a stable, minimal, and explicit input model.

## Current State
- Canonical raw transport input is `RawEnvelope`.
- `RawEnvelope` carries the three domain metadata fields plus payload source information.
- `EnvelopeBase` is the minimal shared base for envelope metadata.
- `EventEnvelopeBase` remains the compatibility layer above `EnvelopeBase`.
- `ResolvingExampleEvent` exists only as a compatibility type built on top of `RawEnvelope`.
- The current model is intentionally small and does not reintroduce `IEvent`.

## What This Unit Owns
- Envelope shape and field set.
- Metadata completeness for transport-to-materialization handoff.
- Compatibility behavior for older example-oriented envelope types.
- Rules for which fields are considered transport metadata versus payload-bearing data.

## What This Unit Does Not Own
- Payload resolution logic.
- Materialization pipeline behavior.
- Handling pipeline order and consumer steps.
- DI composition.
- Publish path.
- Operational policy such as retry, dead-letter, idempotency, or telemetry.

## Projects in Scope
- `src/Models`
- `src/Contracts`
- `tests/CommonMessaging.UnitTests`
- `tests/CommonMessaging.ContractTests`

## Tests in Scope
Primary tests:
- `tests/CommonMessaging.UnitTests/EnvelopeBaseTests.cs`
- `tests/CommonMessaging.UnitTests/EnvelopeMaterializerTests.cs`
- `tests/CommonMessaging.ContractTests/ContractSmokeTests.cs`

Why these tests:
- `EnvelopeBaseTests` validates the base envelope shape and compatibility behavior.
- `EnvelopeMaterializerTests` verifies the metadata copied from the raw envelope into the typed envelope boundary.
- `ContractSmokeTests` ensures the public materializer contract remains stable for consumers.

## Minimal Test Scope Guidance
When working only in this unit, run the smallest possible set first:
1. `CommonMessaging.UnitTests` for envelope-related unit coverage.
2. `CommonMessaging.ContractTests` for public contract stability.

Run broader test projects only if the change crosses into another unit, especially materialization, handling, or DI composition.

## Acceptance Criteria
- Raw envelope shape remains minimal and explicit.
- Envelope metadata remains stable and easy to reason about.
- Compatibility type stays compatibility-only.
- Changes can be validated without full solution execution.

## Out of Scope for This Step
- Any publish-side design.
- Any operational policy design.
- Any pipeline extensibility beyond envelope boundaries.
- Any consumer simulation scenarios.
