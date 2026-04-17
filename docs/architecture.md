# Architecture

## Purpose
Provide a maintainable and scalable foundation for a multi-project .NET 8 library repository.

## High-Level Boundaries
- `src/`: production libraries only.
- `tests/`: automated tests mirroring `src/` structure.
- `samples/`: minimal usage examples.
- `docs/`: architecture and delivery governance.

## Structural Principles
- Keep projects cohesive and focused on a single responsibility.
- Prefer explicit dependencies over hidden coupling.
- Enforce stable contracts at project boundaries.
- Keep cross-project references minimal and intentional.

## Build and Quality Baseline
- Shared build defaults from `Directory.Build.props`.
- Central package versioning from `Directory.Packages.props`.
- Analyzer-driven quality gates enabled in build.

## Future Multi-Project Layout (Proposed)
- `src/ArchiScrapper.Core`
- `src/ArchiScrapper.Infrastructure`
- `src/ArchiScrapper.<Feature>`
- `tests/ArchiScrapper.Core.Tests`
- `tests/ArchiScrapper.Infrastructure.Tests`

Add projects only when a concrete requirement exists.
