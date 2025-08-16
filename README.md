
# ArchiScraper

**ArchiScraper** is a collection of GitHub Copilot prompts designed to **reconstruct and document system architecture**.

This repository acts as a **tooling add-on**:
- provides prompts and helper files describing architectural structures,  
- can be attached to other repositories (e.g., individual microservices),  
- enables you to paste prompts and iteratively rebuild the architecture of a given module **directly within its context**.  

As a result, you can maintain a **consistent, step-by-step architecture map**, stored alongside the source code.

---

## Repository Structure

```

/ArchiScraper
│── README.md            # Project description and usage guidelines
│── /prompts             # Ready-to-use prompt files (H\_\* for humans, C\_\* for Copilot)
│── /landscape           # Full system map (all microservices and their relations)
│── /copilot\_lab         # Copilot workspace: configs, temporary or helper files

````

### Naming convention
- Files optimized for **humans** must start with the prefix **H\_**  
  (e.g., `H_Guide.md`).  
- Files optimized for **Copilot** must start with the prefix **C\_**  
  (e.g., `C_Documentation_Prompt.md`).  

This ensures the repository remains equally clear for both **developers** and **Copilot**.

---

## Policy: *NOOB DRIVEMENT DOCUMENTATION*

While the **architecture** is primarily created and maintained by senior engineers,  
the **documentation** must be written so that even a **beginner developer** can easily follow it.  

The idea is simple:
- **Senior time is expensive** → avoid wasting it on trivial explanations.  
- **Junior-friendly documentation** → ensures knowledge transfer, smooth onboarding, and better long-term maintainability.  

This principle drives all documentation within **ArchiScraper**.

---

## Setup Instructions

If the folder structure does not exist, create it manually:

```bash
# Clone repository
git clone <repo-url>
cd ArchiScraper

# Create required folders
mkdir prompts
mkdir landscape
mkdir copilot_lab
````

You can then start adding files following the **H\_ / C\_ naming convention**.

---

## Landscape folder

The `/landscape` folder represents the entire system.

Its structure is as follows:

```
/landscape
│── Architecture.md          # Global system architecture: how all microservices interact
│── /auth-service            # Example microservice folder
│── /payment-service         # Example microservice folder
│── /user-service            # Example microservice folder
│   └── ...                  # Additional files or subfolders describing this service
```

### Rules

* **`Architecture.md`**
  Provides a **top-level view** of the whole system:

  * service interactions,
  * communication flows,
  * dependencies,
  * high-level diagrams or explanations.

* **Microservice folders**
  Each microservice gets its **own folder**, named after the service (e.g. `order-service`).
  These folders may contain:

  * service-specific documentation,
  * Copilot prompt outputs,
  * architectural call maps,
  * or other artifacts generated during analysis.

---

## Microservice folders

Each microservice inside `/landscape` has its own folder, e.g.:

```
/landscape/order-service
│── H_Architecture.md    # Human-readable description of the service’s purpose and design
│── C_IO.yaml            # Copilot-optimized file with inputs/outputs of this service
│── /api                 # Example project within the microservice
│   └── H_ProjectOverview.md
│── /worker              # Another project within the microservice
│   └── H_ProjectOverview.md
```

### Rules

* **`H_Architecture.md`**
  High-level documentation for humans: what the service does, its responsibilities, and design assumptions.

* **`C_IO.yaml`** (or `.json`)
  Machine-friendly definition of service inputs/outputs, optimized for Copilot:

  * Events consumed (and their origin)
  * APIs exposed (and who consumes them)
  * Other integration points

* **Project subfolders**
  Each project within the microservice (e.g. API, workers, schedulers, frontends) should have its own folder with at least:

  * `H_ProjectOverview.md` describing its role, purpose, and key aspects.
