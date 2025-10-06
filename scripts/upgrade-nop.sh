#!/usr/bin/env bash
set -euo pipefail

if [ $# -lt 1 ]; then
  echo "Usage: $0 <nop_tag> [theme_remote] [theme_path] [theme_branch] [upstream_url]"
  echo "Example: $0 4.90.0"
  exit 1
fi

NOP_TAG="$1"
THEME_REMOTE="${2:-git@github.com:TriVectorX/maiba-jewelry-shop-theme.git}"
THEME_PATH="${3:-src/Presentation/Nop.Web/Themes/Maiba}"
THEME_BRANCH="${4:-main}"
UPSTREAM="${5:-https://github.com/nopSolutions/nopCommerce.git}"

echo "Upgrading to nopCommerce tag: $NOP_TAG"
echo "Theme remote: $THEME_REMOTE"
echo "Theme path  : $THEME_PATH"
echo "Theme branch: $THEME_BRANCH"
echo

# 1) Ensure we're in a git repo
git rev-parse --is-inside-work-tree >/dev/null

# 2) Ensure upstream remote
if ! git remote get-url upstream >/dev/null 2>&1; then
  git remote add upstream "$UPSTREAM"
fi

# 3) Fetch upstream tags
git fetch upstream --tags --prune

# 4) Create new release branch from tag
BRANCH="release-$NOP_TAG"
if git rev-parse --verify "refs/heads/$BRANCH" >/dev/null 2>&1; then
  echo "Branch $BRANCH already exists. Checking out..."
  git checkout "$BRANCH"
else
  echo "Creating branch $BRANCH from tags/$NOP_TAG ..."
  git checkout -b "$BRANCH" "tags/$NOP_TAG"
  git push -u origin "$BRANCH"
fi

# 5) Ensure submodule exists & points to THEME_REMOTE
if [ -d "$THEME_PATH/.git" ]; then
  git submodule sync -- "$THEME_PATH"
else
  echo "Adding theme submodule (upgrade flow)..."
  mkdir -p "$(dirname "$THEME_PATH")"
  git submodule add "$THEME_REMOTE" "$THEME_PATH"
  git config -f .gitmodules "submodule.$THEME_PATH.branch" "$THEME_BRANCH"
  git add .gitmodules "$THEME_PATH"
  git commit -m "Add theme submodule for $BRANCH"
  git push
fi

# 6) Init/Update submodules
git submodule update --init --recursive

# 7) Checkout theme branch
pushd "$THEME_PATH" >/dev/null
git remote set-url origin "$THEME_REMOTE" || true
git fetch origin --prune
git checkout -B "$THEME_BRANCH" "origin/$THEME_BRANCH" || git checkout -B "$THEME_BRANCH"
popd >/dev/null

git config diff.submodule log
git config status.submoduleSummary 1

echo
echo "✔ Upgrade branch ready: $BRANCH"
echo "Now adapt/test your theme against $NOP_TAG, then commit theme changes and bump the submodule pointer."
