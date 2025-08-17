# OnCodeRefactor

## Purpose
Defines how Copilot should perform refactoring across the microservices repo, at both project/module level and service architecture level.

Goals:
1. Identify repeating code fragments.  
2. Detect common design patterns for reuse.  
3. Find classes/methods/structures violating SOLID.  
4. Recommend improvements for readability, modularity, maintainability.

---

## 1. Repo Scan
- Copilot scans project by project, module by module.  
- At microservice level → scans all source files.  
- At architecture level → consolidates repeated patterns and reusable code across services.

---

## 2. Repeated Elements
- **Within microservice:**
  - Detect fragments repeated ≥3 times.
  - Identify methods/classes extractable into base/abstract classes.
  - Save candidates in service folder.
- **Across architecture:**
  - Consolidate recurring patterns across services.
  - Record their locations.
  - Evaluate extraction into tools/shared libs.

---

## 3. Design Patterns
- Detect known design patterns during scan.  
- If pattern is local → record in microservice.  
- If in multiple services → record in architecture folder with locations.

---

## 4. SOLID Violations
Analyze classes/methods for:
- Single Responsibility  
- Open/Closed  
- Liskov Substitution  
- Interface Segregation  
- Dependency Inversion  

Record violations and propose simplifications/refactors.

---

## 5. Consolidation & Recommendations
After scanning all modules:
- Consolidate repeated fragments.  
- Consolidate common patterns across services.  
- Generate recommendations:
  - Refactor SOLID violations
  - Extract repeats into tools/shared libs
  - Improve readability & modularity

Report in architecture folder must list all findings + proposed changes.

---

## 6. Copilot Priorities
1. Identify repeats & SOLID issues within microservice.  
2. Analyze cross-service recurring patterns.  
3. Recommend shared tools + readability improvements.  
4. All refactor suggestions require human review before changes.

---

## Outcome
- Map of repeated code in microservices.  
- Identified SOLID violations + fixes.  
- Detected reusable design patterns.  
- Report ready for human evaluation & implementation.