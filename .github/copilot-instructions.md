# Copilot Instructions for This Repository

## Scope and Priority
- Focus on repository foundation, architecture hygiene, and maintainable .NET 8 library setup.
- Do not introduce business domain logic unless explicitly requested.
- Do not introduce messaging contracts unless explicitly requested.

## Current Implementation Baseline (2026-04-19)
- Keep the message model intentionally simple.
- Do not reintroduce `IEvent` unless explicitly requested.
- Canonical transport input for processing flow is `RawEnvelope`.
- `ResolvingExampleEvent` exists only as a compatibility type built on top of `RawEnvelope`.
- Materialization path supports payload source resolution: inline payload and payload reference.
- Core processing path is: `RawEnvelope` -> materialization pipeline -> typed envelope -> handling pipeline.
- Handling pipeline preserves framework/business split:
	- Infrastructure steps (`IInfrastructureStep<TPayload>`) for framework-owned technical processing.
	- Business steps (`IBusinessStep<TPayload>`) for domain/application-level policy.
	- Consumer handler (`IEventConsumer<TPayload>`) as final business endpoint.
- Maintain multi-layer test structure and keep it green:
	- Unit, Contract, Integration, E2E, Compatibility, Performance.

## Continuity Protocol
- When architectural or behavioral decisions are made, update `docs/decisions.md`.
- When implementation scope changes, update `docs/changelog.md` and any relevant plan/status doc.
- Keep repository memory in sync by appending concise, factual notes under `/memories/repo/`.
- Prefer documenting stable decisions and current baseline, not transient terminal noise.

## Facade Architecture Constraint
This project is a domain-specific facade over an existing internal messaging library.

Layer model:
- Layer 1 (existing, external): generic messaging infrastructure, broker abstraction, serialization, transport, and delivery mechanics.
- Layer 2 (this repository): domain messaging model, message classification (Event vs DataMessage), conventions, and developer-facing API.

This repository is responsible for:
- Domain-specific messaging abstractions.
- Simplified and opinionated API for application teams.
- Message type classification and usage conventions.
- Developer experience layer for consistent usage.

This repository is not responsible for:
- Transport or broker integration implementation.
- Low-level serialization implementation (unless explicitly exposed as configuration).
- Retry, delivery guarantee, and failure mechanics implementation (assume handled by Layer 1 unless confirmed otherwise).

Default behavior for unclear requirements:
- First verify whether the concern is already handled by the underlying messaging library.
- Prefer delegation over reimplementation.
- If unclear or potentially redundant, stop and ask for clarification before implementation.

## Language Rules
- Human communication: Polish.
- Source code, technical comments, XML docs, file names, and commit-ready artifacts: English.

## Engineering Principles
- Prefer simple and explicit designs over clever abstractions.
- Optimize for long-term maintainability and team collaboration.
- Keep changes minimal, cohesive, and production-ready.
- Avoid speculative implementation.

## Permanent Coding Priorities
Priorities order:
1. Correct abstraction over the underlying messaging library
2. SOLID and Clean Code
3. Simplicity of developer API
4. Consistency
5. Extensibility
6. Performance (when justified)
7. Backward compatibility (future phase)

## SOLID Enforcement (Highest Priority)
For every code and architecture decision, apply SOLID practically:
- Single Responsibility Principle
- Open/Closed Principle
- Liskov Substitution Principle
- Interface Segregation Principle
- Dependency Inversion Principle

Before proposing implementation, validate internally:
- Does each class have one reason to change?
- Can stable code be extended without modification?
- Are abstractions substitutable without breaking behavior?
- Are interfaces minimal and purpose-focused?
- Are dependencies inverted toward abstractions?

If any answer is weak, redesign before generating output.

## Clean Code Enforcement (Second Highest Priority)
Prefer:
- Readable code over clever code.
- Explicit intent over shortcuts.
- Small focused classes and methods.
- Descriptive naming.
- Low coupling and high cohesion.
- Testable design and dependency injection.
- Clear boundaries and simple flow.

Avoid:
- God classes.
- Long methods.
- Hidden side effects.
- Boolean flag methods.
- Deep nesting.
- Duplicated logic.
- Unclear naming.
- Premature optimization.
- Static global state.
- Mixed responsibilities.
- Speculative abstractions.

## Design Review Mindset
Act as a senior reviewer first, generator second.

Before finalizing output, validate:
- Is this clean?
- Is this simpler?
- Is responsibility clear?
- Is naming obvious?
- Is dependency direction correct?
- Is this easily testable?
- Would an experienced architect accept this?

If quality is not strong, improve first.

## Refactoring Behavior
When existing code is messy:
1. Preserve behavior.
2. Improve readability.
3. Improve structure.
4. Reduce coupling.
5. Increase cohesion.
6. Remove smells safely.

## Communication Expectations
When suggesting implementation, explain briefly:
- Why the design follows SOLID.
- Why the approach is cleaner than alternatives.
- What future maintenance problems it prevents.

## .NET Standards
- Target .NET 8 unless a task requires otherwise.
- Use nullable reference types and implicit usings.
- Keep analyzers enabled and treat warnings as quality signals.
- Use Central Package Management via Directory.Packages.props.

## Repository Conventions
- `src/` contains production libraries only.
- `tests/` contains automated tests mirroring `src/` structure.
- `samples/` contains minimal usage examples.
- `docs/` stores architecture, decisions, roadmap, backlog, glossary, and changelog.

## Pull Request Quality Bar
- Include rationale for non-trivial decisions.
- Keep diffs focused and avoid unrelated refactors.
- Add or update tests when behavior changes.
- Update docs if architecture, standards, or workflow changes.
