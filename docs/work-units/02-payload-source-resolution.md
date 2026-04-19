# Work Unit 02: Payload Source Resolution

## Goal
Define how the framework resolves the payload source before materialization, so the system can support both inline payloads and payload references in a predictable way.

## Current State
- `RawEnvelope` carries payload source information through `Payload` and `PayloadReference`.
- `IPayloadSourceResolver` is the boundary that decides which payload source to use.
- `PayloadSourceResolver` resolves inline payloads when no reference is present.
- `PayloadSourceResolver` resolves referenced payloads through `IPayloadStorageProvider` when a reference exists.
- `InMemoryPayloadStorageProvider` exists as the default lightweight storage implementation for tests and local composition.
- `EnvelopeMaterializationPipeline` uses the resolver boundary before typed payload creation.
- The default DI composition wires the resolver and storage provider together.

## What This Unit Owns
- Inline vs reference payload source selection.
- Storage lookup boundary for referenced payloads.
- Default resolution behavior used by materialization.
- The contract that payload resolution happens before typed materialization.

## What This Unit Does Not Own
- Envelope shape and base metadata contract.
- Typed envelope materialization details beyond source selection.
- Handling pipeline order and custom steps.
- DI composition beyond resolver wiring.
- Publish path.
- Retry, dead-letter, idempotency, telemetry, or other operational policy.

## Projects in Scope
- `src/Messaging.Abstractions`
- `src/Messaging.Core`
- `src/Messaging.Extensions`
- `tests/CommonMessaging.UnitTests`
- `tests/CommonMessaging.ConsumerSimulationTests`

## Tests in Scope
Primary tests:
- `tests/CommonMessaging.UnitTests/EnvelopeMaterializationPipelineTests.cs`
- `tests/CommonMessaging.ConsumerSimulationTests/ConsumerSimulationScenariosTests.cs`

Secondary coverage that may become relevant if the boundary changes:
- `tests/CommonMessaging.UnitTests/MessagingExtensionsRegistrationTests.cs`

Why these tests:
- `EnvelopeMaterializationPipelineTests` validates inline and reference resolution directly at the materialization boundary.
- `ConsumerSimulationScenariosTests` verifies that the resolution path works in a realistic consumer-like composition.
- `MessagingExtensionsRegistrationTests` catches resolution regressions caused by DI wiring.

## Minimal Test Scope Guidance
When working only in this unit, run the smallest possible set first:
1. `CommonMessaging.UnitTests` for direct resolver and materialization boundary coverage.
2. `CommonMessaging.ConsumerSimulationTests` if the change touches DI composition or realistic payload-reference usage.

Run broader tests only if the change crosses into envelope shape, handling, or publish policy.

## Acceptance Criteria
- Inline payloads and referenced payloads are resolved deterministically.
- Storage lookup is explicit and isolated behind the boundary.
- Materialization only receives already-selected payload content.
- The unit can be validated with focused tests, not the full solution.

## Out of Scope for This Step
- Any publish-side design.
- Any envelope metadata redesign.
- Any operational policy design.
- Any handling pipeline changes.
- Any consumer simulation behavior unrelated to resolution.
