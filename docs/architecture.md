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
