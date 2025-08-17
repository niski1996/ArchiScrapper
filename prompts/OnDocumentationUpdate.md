# OnDocumentationUpdate

## Purpose
Rules for updating microservice documentation in the central repo. Docs exist in **two layers**:

1. **H_** – human-readable, full architecture + module descriptions
2. **C_** – Copilot-readable, token-optimized minimal info

Updates must keep both layers consistent.

---

## Update Rules

### 1. Temp Folder
- Single shared folder for Copilot temp data (diffs, integrations, commit states).  
- Overwritten with each new module update, no per-module folders.

### 2. Change Identification
- Identify branch + commit.  
- Use changelog to generate diff vs last version.  
- Update only where changes are certain.

### 3. Scope
- Update only:
  - new/modified modules, classes, features affecting integrations/architecture
  - I/O points (events, APIs, DBs)
  - Copilot files (`C_`)
- Do **not** update UML/Mermaid unless sure of relation changes.  
- Ignore trivial code/comment-only changes.

### 4. Human Review
- All updates require human approval before merge.  
- Copilot proposes, human validates, then merges.

### 5. Consistency
- `C_` ⊆ `H_`.  
- `H_` may add extra context but not conflict.  
- All `C_` changes must be reflected in `H_`.

### 6. Changelog
Each doc must include:
```markdown
## Changelog
- <commit-hash>: <YYYY-MM-DD>: <change-description>
```