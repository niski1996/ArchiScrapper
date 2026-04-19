# tests

This folder contains automated tests mirroring the `src/` project structure.

## Rules
# tests

Test projects are split by purpose to protect framework quality without over-focusing on raw coverage percentage.

## Projects

- `CommonMessaging.UnitTests`: Isolated tests for pipeline steps, builders, materialization, and utility behavior.
- `CommonMessaging.ContractTests`: Shared contract suites for extensibility points and provider compliance.
- `CommonMessaging.IntegrationTests`: Multi-component processing flows and framework wiring checks.
- `CommonMessaging.E2ETests`: End-to-end consumer scenarios over full framework setup.
- `CommonMessaging.ConsumerSimulationTests`: Real consumer-like service scenarios focused on framework API ergonomics, configuration usability, and extensibility.
- `CommonMessaging.CompatibilityTests`: API and behavior regression checks for backward compatibility.
- `CommonMessaging.PerformanceTests`: Selected throughput, payload-size, and allocation baselines.

## Execution policy

- Pull Requests: run Unit + Contract + lightweight Integration smoke.
- Nightly: run full Integration + E2E + ConsumerSimulation + Compatibility.
- Release gate: run Compatibility + Performance baseline suite.
