# Work Unit 03: Materialization Pipeline

## Goal
Convert a raw envelope into a typed envelope in a deterministic way, after payload source resolution has already selected the payload content.

## Current State
- `EnvelopeMaterializationPipeline` is implemented as the orchestration layer for raw-to-typed conversion.
- The pipeline validates the raw source metadata before materialization.
- The pipeline resolves payload content through `IPayloadSourceResolver`.
- The pipeline constructs a typed envelope from the resolved payload and the envelope metadata.
- `EnvelopeMaterializer` is the public materialization facade over the pipeline.
- Default DI composition wires the pipeline through the resolver boundary.
- The current implementation is intentionally simple and deterministic, with a fixed default stage sequence.

## What This Unit Owns
- Raw-to-typed conversion flow.
- Validation of source metadata before typed envelope creation.
- Stage sequencing inside materialization.
- The boundary between payload resolution and typed payload construction.
- The public materializer facade that exposes the pipeline.

## What This Unit Does Not Own
- Envelope shape and metadata contract.
- Payload reference storage mechanics.
- Handling pipeline order and custom steps.
- DI composition beyond materialization wiring.
- Publish path.
- Retry, dead-letter, idempotency, or other operational policy.

## Projects in Scope
- `src/Messaging.Abstractions`
- `src/Messaging.Core`
- `src/Messaging.Extensions`
- `tests/CommonMessaging.UnitTests`
- `tests/CommonMessaging.ContractTests`
- `tests/CommonMessaging.IntegrationTests`

## Tests in Scope
Primary tests:
- `tests/CommonMessaging.UnitTests/EnvelopeMaterializationPipelineTests.cs`
- `tests/CommonMessaging.UnitTests/EnvelopeMaterializerTests.cs`
- `tests/CommonMessaging.ContractTests/ContractSmokeTests.cs`

Secondary tests when the DI wiring changes:
- `tests/CommonMessaging.UnitTests/MessagingExtensionsRegistrationTests.cs`
- `tests/CommonMessaging.IntegrationTests/IntegrationSmokeTests.cs`

Why these tests:
- `EnvelopeMaterializationPipelineTests` validates the pipeline behavior directly, including inline and reference payload paths.
- `EnvelopeMaterializerTests` validates the public facade over the pipeline.
- `ContractSmokeTests` validates the public contract boundary for consumers.
- `MessagingExtensionsRegistrationTests` catches composition regressions when the default pipeline wiring changes.
- `IntegrationSmokeTests` confirms the materialization path still works inside the wider processing flow.

## Minimal Test Scope Guidance
When working only in this unit, run the smallest possible set first:
1. `CommonMessaging.UnitTests` for direct pipeline and facade coverage.
2. `CommonMessaging.ContractTests` for public contract stability.
3. `CommonMessaging.IntegrationTests` only if the DI wiring or the processing-flow handshake changes.

Run broader tests only if the change crosses into handling, consumer simulation, or envelope metadata.

## Acceptance Criteria
- Raw envelope is converted to typed envelope deterministically.
- Payload source resolution happens before typed materialization.
- Pipeline stage ordering is stable and explicit.
- The public facade remains thin and easy to consume.
- Changes can be validated with a narrow test slice.

## Out of Scope for This Step
- Any publish-side design.
- Any envelope metadata redesign.
- Any operational policy design.
- Any handling pipeline changes.
- Any consumer simulation behavior unrelated to materialization.
