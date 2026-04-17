---
mode: ask
description: "Refactor existing .NET code for maintainability while preserving behavior and minimizing risk."
---

# Refactor

Analyze the target code and provide a safe refactoring plan.

## Required Output
1. Current issues (complexity, coupling, naming, testability).
2. Refactoring steps in safe order.
3. Behavior preservation checks.
4. Test updates needed.
5. Rollback strategy for risky steps.

## Rules
- Preserve public behavior unless explicitly approved otherwise.
- Prefer incremental commits.
- Do not mix refactoring with unrelated feature work.
