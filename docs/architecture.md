# Architecture

## Purpose
Provide a maintainable and scalable foundation for a multi-project .NET 8 library repository.

## Architecture Role
This project is a domain-specific facade over an existing internal messaging library.

Layering model:
- Layer 1 (external): generic messaging infrastructure, broker abstraction, serialization, transport, and delivery mechanics.
- Layer 2 (this project): domain messaging model, Event/DataMessage classification, conventions, and developer-facing API.

Design implication:
- Prefer delegation to Layer 1 capabilities.
- Do not duplicate infrastructure concerns already solved by Layer 1.

## High-Level Boundaries
- `src/`: production libraries only.
- `tests/`: automated tests mirroring `src/` structure.
- `samples/`: minimal usage examples.
- `docs/`: architecture and delivery governance.

Responsibility boundary:
- In scope: domain abstractions, conventions, and DX API.
- Out of scope: transport implementation, broker mechanics, low-level delivery/retry internals.

## Structural Principles
- Keep projects cohesive and focused on a single responsibility.
- Prefer explicit dependencies over hidden coupling.
- Enforce stable contracts at project boundaries.
- Keep cross-project references minimal and intentional.

## Build and Quality Baseline
- Shared build defaults from `Directory.Build.props`.
- Central package versioning from `Directory.Packages.props`.
- Analyzer-driven quality gates enabled in build.

## Engineering Priorities
Priority order for design and implementation decisions:
1. Correct abstraction over the underlying messaging library
2. SOLID and Clean Code
3. Simplicity of developer API
4. Consistency
5. Extensibility
6. Performance (when justified)
7. Backward compatibility (future phase)

## SOLID Application Rules
Apply SOLID as a practical decision filter for classes, abstractions, interfaces, modules, and API boundaries.

Validation checklist before implementation:
- Does each class have one reason to change?
- Can behavior be extended without modifying stable code?
- Are abstractions substitutable without breaking behavior?
- Are interfaces small and purpose-focused?
- Are dependencies inverted toward abstractions?

If any answer is weak, redesign before implementation.

## Clean Code Rules
Prefer:
- Readability over cleverness.
- Explicit intent over shortcuts.
- Small focused classes and methods.
- Descriptive naming.
- Low coupling and high cohesion.
- Testable design with dependency injection.
- Clear boundaries and simple flow.

Avoid:
- God classes and mixed responsibilities.
- Long methods and deep nesting.
- Hidden side effects and boolean flag methods.
- Duplicated logic and unclear naming.
- Static global state.
- Premature optimization and speculative abstractions.

## Design Review Checklist
Treat every implementation as review-first work:
- Is this cleaner than the previous version?
- Is responsibility clear and naming obvious?
- Is dependency direction correct?
- Is the design easy to test?
- Would an experienced architect approve this change?

If quality is not strong, improve before merge.

## Refactoring Policy
When improving existing code:
1. Preserve behavior.
2. Improve readability.
3. Improve structure.
4. Reduce coupling.
5. Increase cohesion.
6. Remove code smells safely.

## Future Multi-Project Layout (Proposed)
- `src/ArchiScrapper.Core`
- `src/ArchiScrapper.Infrastructure`
- `src/ArchiScrapper.<Feature>`
- `tests/ArchiScrapper.Core.Tests`
- `tests/ArchiScrapper.Infrastructure.Tests`

Add projects only when a concrete requirement exists.

## Architecture Completion Map

The current baseline is strongest on the consumption path. The remaining architecture should be completed in four high-level workstreams:

1. Publish path
- Define the publish lifecycle from typed model to outbound transport message.
- Keep publishing boundaries explicit: serialization, payload selection, transformation, storage, envelope composition, handoff to Layer 1.
- Separate publishing policy from transport mechanics.

2. Metadata and contract policy
- Define the authoritative envelope metadata set for identity, correlation, causation, versioning, payload location, and observability.
- Decide which metadata is mandatory, optional, or transport-owned.
- Establish compatibility expectations for contract evolution.

3. Operational policy
- Define failure classification, retry, dead-letter, poison-message handling, and idempotency expectations.
- Clarify which operational concerns belong to this facade versus Layer 1.
- Keep cancellation and telemetry semantics consistent across publish and consume flows.

4. Extensibility and developer experience
- Keep custom steps, storage, encryption, compression, and serialization as explicit extension points.
- Preserve simple defaults for common usage and allow advanced policy only when needed.
- Ensure consumer simulation remains the acceptance-level view of the framework.

## Suggested Independent Work Units

To keep implementation iterative and avoid running the full test suite for every change, the architecture should be worked in small units with explicit dependency boundaries.

### 1. Envelope and metadata contract
- Scope: canonical raw transport envelope, envelope metadata, contract stability, compatibility model.
- Depends on: nothing else in the framework.
- Primary validation: model-level unit tests and contract tests.
- Typical change size: small and stable.

### 2. Payload source resolution boundary
- Scope: inline payload vs payload reference, payload lookup policy, storage abstraction boundary.
- Depends on: envelope and metadata contract.
- Primary validation: unit tests and contract tests around payload source behavior.
- Typical change size: small to medium.

### 3. Materialization pipeline
- Scope: raw envelope to typed envelope conversion, resolver integration, payload shaping.
- Depends on: envelope contract and payload resolution boundary.
- Primary validation: unit tests plus focused integration tests.
- Typical change size: medium.

### 4. Handling pipeline and custom steps
- Scope: infrastructure steps, business steps, consumer handler order, pipeline composition.
- Depends on: typed envelope and handling context.
- Primary validation: unit tests and consumer-simulation tests.
- Typical change size: medium.

### 5. Dependency injection and composition
- Scope: service registration, default wiring, host composition, extension methods.
- Depends on: materialization and handling contracts.
- Primary validation: integration tests and consumer-simulation tests.
- Typical change size: medium.

### 6. Consumer simulation scenarios
- Scope: real consumer-like service setup, usability, ergonomics, multi-service isolation.
- Depends on: DI and composition, handling pipeline, payload materialization.
- Primary validation: consumer-simulation tests.
- Typical change size: medium to large, but isolated.

### 7. Publish path
- Scope: outbound message composition, envelope finalization, payload selection, handoff to transport layer.
- Depends on: envelope contract and metadata policy.
- Primary validation: publish-focused unit and integration tests once publishing is in scope.
- Typical change size: large, and should be worked separately from consume-path changes.

### 8. Operational policy
- Scope: retry, dead-letter, idempotency, cancellation, telemetry, error classification.
- Depends on: both publish and consume paths.
- Primary validation: targeted integration and scenario tests.
- Typical change size: large and cross-cutting.

## Test Scope Guidance

- If the change touches only one unit above, run only that unit’s test layer plus direct dependents.
- If the change crosses a boundary, run the dependent unit and one adjacent layer upward.
- Full solution test runs should be reserved for:
	- contract changes that affect multiple units,
	- DI/composition changes,
	- any operational-policy change,
	- broad refactors touching shared abstractions.

## Work Unit Matrix

| Unit | Depends On | Smallest Useful Test Scope | Avoid Touching |
| --- | --- | --- | --- |
| Envelope and metadata contract | Nothing | Unit + Contract | Pipeline behavior, DI wiring |
| Payload source resolution | Envelope contract | Unit + Contract | Handling order, publish policy |
| Materialization pipeline | Envelope + payload resolution | Unit + focused Integration | Publish path, operational policy |
| Handling pipeline and custom steps | Typed envelope + handling context | Unit + ConsumerSimulation | Publish path, storage mechanics |
| DI and composition | Materialization + handling contracts | Integration + ConsumerSimulation | Contract semantics, metadata schema |
| Consumer simulation | DI + materialization + handling | ConsumerSimulation only | Internal step mechanics |
| Publish path | Envelope + metadata policy | Publish-focused Unit + Integration | Consumer-specific acceptance scenarios |
| Operational policy | Publish + consume paths | Targeted Integration + scenario tests | Low-level model shape |
