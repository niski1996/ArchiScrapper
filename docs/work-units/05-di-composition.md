# Work Unit 05: DI and Composition

## Goal
Define how the framework is composed through dependency injection so consumers can wire the core messaging flow with a small, explicit, and predictable setup.

## Current State
- `AddCommonMessagingCore()` registers the payload storage provider, payload source resolver, materialization pipeline, and materializer.
- `AddRawEventProcessingFlow<TPayload>()` registers the handling pipeline and the raw processing flow.
- The DI wiring currently depends on the materialization, handling, and consumer extension boundaries already defined in earlier units.
- The composition layer is intentionally minimal and keeps framework wiring separate from business steps and consumer handlers.

## What This Unit Owns
- Default service registration for the framework.
- Construction of the overall processing flow from registered building blocks.
- The boundary between framework-owned services and consumer-owned services.
- The user-facing composition surface for application teams.

## What This Unit Does Not Own
- Envelope shape and metadata contract.
- Payload source resolution logic.
- Materialization pipeline behavior.
- Handling pipeline step ordering and custom steps.
- Publish path.
- Retry, dead-letter, idempotency, or other operational policy.

## Projects in Scope
- `src/Messaging.Extensions`
- `src/Messaging.Core`
- `src/Messaging.Abstractions`
- `tests/CommonMessaging.UnitTests`
- `tests/CommonMessaging.IntegrationTests`
- `tests/CommonMessaging.ConsumerSimulationTests`
- `tests/CommonMessaging.CompatibilityTests`

## Tests in Scope
Primary tests:
- `tests/CommonMessaging.UnitTests/MessagingExtensionsRegistrationTests.cs`
- `tests/CommonMessaging.IntegrationTests/IntegrationSmokeTests.cs`
- `tests/CommonMessaging.ConsumerSimulationTests/ConsumerSimulationScenariosTests.cs`

Secondary coverage:
- `tests/CommonMessaging.UnitTests/RawEventProcessingFlowBuilderTests.cs`
- `tests/CommonMessaging.UnitTests/RawEventProcessingFlowTests.cs`
- `tests/CommonMessaging.CompatibilityTests/CompatibilitySmokeTests.cs`

Why these tests:
- `MessagingExtensionsRegistrationTests` validates that default DI composition produces an executable flow.
- `IntegrationSmokeTests` confirms the composed flow still works in the broader processing path.
- `ConsumerSimulationScenariosTests` validates user-facing configuration ergonomics and composition independence.
- `RawEventProcessingFlowBuilderTests` and `RawEventProcessingFlowTests` cover builder-level flow wiring that DI ultimately relies on.
- `CompatibilitySmokeTests` guards the public builder/registration contract shape.

## Minimal Test Scope Guidance
When working only in this unit, run the smallest possible set first:
1. `CommonMessaging.UnitTests` for registration and builder wiring.
2. `CommonMessaging.IntegrationTests` if the registration affects flow execution.
3. `CommonMessaging.ConsumerSimulationTests` if the change affects consumer-facing composition or service isolation.
4. `CommonMessaging.CompatibilityTests` only if public registration or builder signatures change.

Run broader tests only if the change crosses into handling semantics, envelope metadata, or payload resolution.

## Acceptance Criteria
- Consumers can register and resolve the default processing flow with a small setup.
- Default registrations remain stable and explicit.
- Composition changes can be validated with a narrow test slice.
- The DI surface remains simple and does not leak internal wiring details.

## Out of Scope for This Step
- Any publish-side design.
- Any envelope metadata redesign.
- Any payload resolution redesign.
- Any handling pipeline semantics change.
- Any operational policy design.
