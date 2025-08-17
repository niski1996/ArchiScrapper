# OnDocumentationCreate

## Purpose
Rules for creating and updating documentation in the central repo for microservices and modules. Docs exist in **two layers**:

1. **Human-readable (H_)** – descriptive, with Mermaid UML diagrams.  
2. **Copilot-readable (C_)** – minimal, token-optimized facts only.

---

## Rules

### Consistency
- All `C_` info must exist in `H_`.  
- `H_` may add extra context but never conflict.

### Scope
- **Module:**
  - `H_<module>.md` – goals, deps, usage, UML diagrams
  - `C_<module>.yaml|json` – minimal facts
- **Microservice:**
  - `H_Architecture.md` – goals, flows, integrations, UML
  - `C_IO.yaml|json` – I/O, deps, integrations

### Changelog
Each doc must include:
```markdown
## Changelog
- Commit: <hash>
- Updated: <date>
```

---

## Integrations (`C_IO.yaml|json`)
- Events in: type, source, schema
- Events out: type, target, schema
- APIs used
- APIs exposed
- DBs (read/write)
- Other (queues, messaging, cache)

---

## Process
1. Select module
2. Generate module docs (`H_<module>.md`, `C_<module>.yaml|json`)
3. Update service docs (`H_Architecture.md`, `C_IO.yaml|json`)
4. Human review → approve → merge

---

## Outcome
- Each service has updated integration map.  
- `H_` = context + UML.  
- `C_` = minimal machine facts.  
- Architecture remains consistent & analyzable.