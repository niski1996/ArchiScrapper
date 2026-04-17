---
mode: ask
description: "Run a risk-first code review for .NET changes, focusing on bugs, regressions, and missing tests."
---

# Review

Review the provided changes with production-readiness criteria.

## Required Output
1. Findings ordered by severity with file references.
2. Regression risks and impacted behaviors.
3. Missing or weak tests.
4. Suggested concrete fixes.
5. Short final quality verdict.

## Rules
- Prioritize correctness and safety over style preferences.
- Be explicit about assumptions.
- If no critical findings exist, state residual risks.
