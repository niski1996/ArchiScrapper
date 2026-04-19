# ArchiScrapper

Foundation-first repository for a .NET 8 library ecosystem.

## Current Stage
The repository now contains a working baseline of the messaging facade with tests and CI:
- canonical processing input based on `RawEnvelope`,
- payload source resolution (inline and reference),
- materialization and handling pipelines,
- publish-side composition and transport handoff abstractions,
- multi-layer automated tests and CI workflows.

The scope stays intentionally focused on framework/facade behavior without domain-specific business logic.

## Repository Structure
- `.github/` AI instructions and reusable prompts.
- `docs/` architecture and planning documentation.
- `src/` production projects.
- `tests/` multi-layer automated tests.
- `samples/` minimal usage examples.

## Engineering Baseline
- .NET 8 conventions via `Directory.Build.props`.
- Centralized NuGet versioning via `Directory.Packages.props`.
- Unified formatting and style rules via `.editorconfig`.
- Standard ignore rules via `.gitignore`.
- CI gates for PR, nightly, and release validation under `.github/workflows`.

## 1.0 User Testing Baseline
- Current package/application version baseline is `1.0.0`.
- Recommended validation command:
	- `dotnet build ArchiScrapper.sln && dotnet test ArchiScrapper.sln --no-build`
- Current implementation details and compatibility notes are tracked in:
	- `docs/current-state.md`
	- `docs/decisions.md`
	- `docs/changelog.md`
