# Work Unit 04: Handling Pipeline and Custom Steps

## Goal
Define how the framework executes infrastructure steps, business steps, and the final consumer handler in a stable and composable order.

## Current State
- `HandlingPipeline<TPayload>` executes infrastructure steps first, then business steps, then the consumer handler.
- `HandlingPipelineBuilder<TPayload>` is the current composition entry point for building the pipeline.
- `IInfrastructureStep<TPayload>` and `IBusinessStep<TPayload>` define the extension boundary for custom framework and business logic.
- `IEventConsumer<TPayload>` is the final typed consumer endpoint.
- `HandleContext<TPayload>` carries the typed envelope and a mutable item bag for step coordination.
- `RawEventProcessingFlow<TPayload>` invokes the handling pipeline after materialization.
- The current model is intentionally simple: fixed step groups, explicit ordering, and no hidden orchestration beyond the pipeline itself.

## What This Unit Owns
- Ordering and orchestration of handling steps.
- The split between infrastructure-owned and business-owned steps.
- The final handoff to the consumer handler.
- Shared context passed through the pipeline.
- Builder-level composition of the pipeline.

## What This Unit Does Not Own
- Envelope shape and metadata contract.
- Payload source resolution.
- Materialization pipeline behavior.
- DI composition beyond flow registration.
- Publish path.
- Retry, dead-letter, idempotency, or other operational policy.

## Projects in Scope
- `src/Messaging.Abstractions`
- `src/Messaging.Core`
- `src/Messaging.Extensions`
- `tests/CommonMessaging.UnitTests`
- `tests/CommonMessaging.E2ETests`
- `tests/CommonMessaging.ConsumerSimulationTests`
- `tests/CommonMessaging.CompatibilityTests`

## Tests in Scope
Primary tests:
- `tests/CommonMessaging.UnitTests/HandlingPipelineTests.cs`
- `tests/CommonMessaging.UnitTests/RawEventProcessingFlowTests.cs`
- `tests/CommonMessaging.UnitTests/RawEventProcessingFlowBuilderTests.cs`
- `tests/CommonMessaging.E2ETests/E2ESmokeTests.cs`
- `tests/CommonMessaging.ConsumerSimulationTests/ConsumerSimulationScenariosTests.cs`

Secondary coverage:
- `tests/CommonMessaging.CompatibilityTests/CompatibilitySmokeTests.cs`
- `tests/CommonMessaging.UnitTests/MessagingExtensionsRegistrationTests.cs`

Why these tests:
- `HandlingPipelineTests` validates the ordering contract and builder behavior directly.
- `RawEventProcessingFlowTests` and `RawEventProcessingFlowBuilderTests` confirm the handling pipeline remains correctly wired into the broader flow.
- `E2ESmokeTests` verifies a realistic flow with infrastructure and business steps plus the final consumer.
- `ConsumerSimulationScenariosTests` validates consumer-facing ergonomics and multi-service isolation.
- `CompatibilitySmokeTests` ensures the public builder contract remains stable.

## Minimal Test Scope Guidance
When working only in this unit, run the smallest possible set first:
1. `CommonMessaging.UnitTests` for pipeline and builder behavior.
2. `CommonMessaging.E2ETests` when the change affects public flow composition.
3. `CommonMessaging.ConsumerSimulationTests` when the change affects consumer-facing usability or realistic composition.
4. `CommonMessaging.CompatibilityTests` only if public builder or pipeline contract changes.

Run broader tests only if the change crosses into envelope metadata, payload resolution, or DI wiring.

## Acceptance Criteria
- Infrastructure steps execute before business steps.
- Business steps execute before the final consumer.
- Step continuation behavior remains deterministic.
- Builder composition remains stable and easy to consume.
- The pipeline can be validated with a narrow test slice.

## Out of Scope for This Step
- Any publish-side design.
- Any envelope metadata redesign.
- Any payload resolution redesign.
- Any operational policy design.
- Any storage provider changes.
