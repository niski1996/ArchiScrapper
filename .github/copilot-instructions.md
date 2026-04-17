# Copilot Instructions for This Repository

## Scope and Priority
- Focus on repository foundation, architecture hygiene, and maintainable .NET 8 library setup.
- Do not introduce business domain logic unless explicitly requested.
- Do not introduce messaging contracts unless explicitly requested.

## Language Rules
- Human communication: Polish.
- Source code, technical comments, XML docs, file names, and commit-ready artifacts: English.

## Engineering Principles
- Prefer simple and explicit designs over clever abstractions.
- Optimize for long-term maintainability and team collaboration.
- Keep changes minimal, cohesive, and production-ready.
- Avoid speculative implementation.

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
