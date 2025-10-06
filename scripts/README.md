# Maiba Jewelry Shop — Theme Development Guide

This repo (`TriVectorX/maiba-jewelry-shop`) is the **parent** repository that hosts the nopCommerce **source code** and links the **Maiba theme** as a **Git submodule**:

```
maiba-jewelry-shop (parent repo; your fork/mirror of nopCommerce)
└─ src/Presentation/Nop.Web/Themes/Maiba  ← submodule → TriVectorX/maiba-jewelry-shop-theme
```

The setup lets us:

* Keep upstream nopCommerce code clean and up-to-date (by tag/branch).
* Develop the theme in its own repository with clean history and permissions.
* Pin exact theme commits in the parent repo for reproducible builds.

---

## Prerequisites

* Git (SSH keys configured for GitHub).
* macOS/Linux Bash **or** Windows PowerShell 7+.
* Access to both repos:

  * Parent (this repo): `git@github.com:TriVectorX/maiba-jewelry-shop.git`
  * Theme: `git@github.com:TriVectorX/maiba-jewelry-shop-theme.git`

> **Versioning note**: The parent repo tracks nopCommerce **release tags** (e.g., `4.80.9`) via branches named `release-x.yy.z`.

---

## Quick Start (Developers)

> If you’re joining the project, this is all you need.

### Option A — One‑liner scripts (recommended)

```bash
# macOS/Linux
git clone git@github.com:TriVectorX/maiba-jewelry-shop.git
cd maiba-jewelry-shop
chmod +x scripts/bootstrap-dev.sh
./scripts/bootstrap-dev.sh
```

```powershell
# Windows PowerShell
git clone git@github.com:TriVectorX/maiba-jewelry-shop.git
cd maiba-jewelry-shop
scripts\bootstrap-dev.ps1
```

What this does:

* Checks out/creates the current release branch (e.g., `release-4.80.9`) from the official tag if needed.
* Initializes the **Maiba** theme submodule.
* Ensures your local theme branch (default: `main`) is ready.

### Option B — Manual bootstrap

```bash
git clone --recurse-submodules git@github.com:TriVectorX/maiba-jewelry-shop.git
cd maiba-jewelry-shop
git checkout release-4.80.9                 # or the current release branch
git submodule update --init --recursive
```

---

## Daily Workflow

1. **Work inside the theme submodule**

```bash
cd src/Presentation/Nop.Web/Themes/Maiba
git checkout -b feat/header-redesign        # or use an existing branch
# ...edit files...
git add .
git commit -m "feat(theme): header redesign"
git push -u origin feat/header-redesign
```

2. **Bump the submodule pointer in the parent repo**

```bash
cd ../../../../..
git add src/Presentation/Nop.Web/Themes/Maiba
git commit -m "chore(theme): bump Maiba submodule pointer"
git push
```

> The pointer commit records the exact theme SHA in the parent repo, ensuring everyone builds the same theme version.

---

## Upgrading nopCommerce (Maintainers)

When a new upstream version ships (e.g., `4.90.0`), create a **new** release branch from the official tag and keep the theme linked.

### Using scripts (recommended)

```bash
# macOS/Linux
chmod +x scripts/upgrade-nop.sh
./scripts/upgrade-nop.sh 4.90.0
```

```powershell
# Windows PowerShell
scripts\upgrade-nop.ps1 -NopTag 4.90.0
```

What this does:

* Fetches upstream tags from `nopSolutions/nopCommerce`.
* Creates/pushes a new branch `release-4.90.0` from the official tag.
* Ensures the **Maiba** theme submodule is present and on the expected branch (`main` by default).
* Leaves you ready to test and adapt the theme.

### Manual outline (for reference)

```bash
git fetch upstream --tags --prune
git checkout -b release-4.90.0 tags/4.90.0
git push -u origin release-4.90.0

git submodule update --init --recursive
# (If needed) re-add submodule:
# git submodule add git@github.com:TriVectorX/maiba-jewelry-shop-theme.git src/Presentation/Nop.Web/Themes/Maiba
# git commit -m "Add theme submodule for 4.90.0" && git push
```

Then adapt the theme and bump the pointer as usual.

---

## Scripts (Summary)

All scripts live in `scripts/` in the parent repo.

* **`bootstrap-dev.sh` / `bootstrap-dev.ps1`**
  Bootstraps a dev machine: checks out the current release branch, inits submodule, sets helpful Git settings.

* **`upgrade-nop.sh` / `upgrade-nop.ps1`**
  Creates a new `release-x.yy.z` branch from an official nopCommerce tag and ensures the theme submodule is wired up.

**Defaults inside scripts**

* Parent repo: `TriVectorX/maiba-jewelry-shop`
* Theme repo: `TriVectorX/maiba-jewelry-shop-theme`
* Theme path: `src/Presentation/Nop.Web/Themes/Maiba`
* Theme branch: `main`

> You can override defaults via script parameters; see the script headers.

---

## Common Commands (Cheat Sheet)

* Pull parent + submodules:

  ```bash
  git pull --recurse-submodules
  git submodule update --init --recursive
  ```
* Update theme to latest remote (use intentionally):

  ```bash
  git submodule update --remote --merge src/Presentation/Nop.Web/Themes/Maiba
  git add src/Presentation/Nop.Web/Themes/Maiba
  git commit -m "chore(theme): bump to latest"
  git push
  ```
* Fix detached HEAD inside theme:

  ```bash
  cd src/Presentation/Nop.Web/Themes/Maiba
  git checkout -b my-feature
  ```
* Windows long paths (one-time in parent):

  ```bash
  git config core.longpaths true
  ```

---

## Branching & Versioning

* Parent repo tracks nopCommerce **release tags** via branches named `release-x.yy.z`.
* Theme repo uses normal Git flow (e.g., `main`, `feat/*`, `fix/*`).
* Each parent release branch pins a **specific theme commit** via the submodule pointer.

---

## FAQ

**Why submodules instead of copying the theme into the parent repo?**
Clear ownership, clean history, separate permissions, reproducible builds via pinned commit SHAs.

**Can I work without the entire nopCommerce source?**
For packaging-only workflows, you can. For day-to-day theme development and debugging, the full source + submodule is far more productive.

**Do we ever modify nopCommerce core?**
No. Keep all customization in the **Maiba theme** (and optional plugins). This keeps upstream upgrades painless.

---

## Troubleshooting

* **Theme folder missing after clone**
  Run: `git submodule update --init --recursive`

* **Colleague sees old theme code**
  Ensure you committed the parent pointer:
  `git add src/Presentation/Nop.Web/Themes/Maiba && git commit -m "bump pointer" && git push`

* **Permission denied (theme repo)**
  Ensure your SSH key has access to `TriVectorX/maiba-jewelry-shop-theme`.

* **Submodule confused/out-of-sync**
  From parent root:

  ```bash
  git submodule status
  git submodule sync --recursive
  git submodule update --init --recursive
  ```

---

## Contributing

* Use conventional commits for clarity, e.g.:

  * `feat(theme): ...`
  * `fix(theme): ...`
  * `chore(theme): ...`
* For parent pointer updates:

  * `chore(theme): bump Maiba submodule pointer`

Open PRs against the relevant `release-x.yy.z` branch in the **parent** repo and normal feature branches in the **theme** repo.

---

## License

* nopCommerce: per upstream license.
* Maiba theme: see `TriVectorX/maiba-jewelry-shop-theme` license.
* This repo’s scripts/docs: per this repo’s license file.

---