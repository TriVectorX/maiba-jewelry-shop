#!/usr/bin/env bash
set -euo pipefail

# Defaults (override via args)
PARENT_REMOTE_DEFAULT="git@github.com:TriVectorX/maiba-jewelry-shop.git"
THEME_REMOTE_SSH_DEFAULT="git@github.com:TriVectorX/maiba-jewelry-shop-theme.git"
THEME_REMOTE_HTTPS_DEFAULT="https://github.com/TriVectorX/maiba-jewelry-shop-theme.git"
THEME_PATH_DEFAULT="src/Presentation/Nop.Web/Themes/Maiba"
THEME_BRANCH_DEFAULT="main"
UPSTREAM_DEFAULT="https://github.com/nopSolutions/nopCommerce.git"
NOP_TAG_DEFAULT=""   # e.g. 4.80.9 or release-4.80.9; empty = stay on current branch

# Args: parentRemote themeRemoteSSH themeRemoteHTTPS themePath themeBranch upstream nopTag
PARENT_REMOTE="${1:-$PARENT_REMOTE_DEFAULT}"
THEME_REMOTE_SSH="${2:-$THEME_REMOTE_SSH_DEFAULT}"
THEME_REMOTE_HTTPS="${3:-$THEME_REMOTE_HTTPS_DEFAULT}"
THEME_PATH="${4:-$THEME_PATH_DEFAULT}"
THEME_BRANCH="${5:-$THEME_BRANCH_DEFAULT}"
UPSTREAM="${6:-$UPSTREAM_DEFAULT}"
NOP_TAG="${7:-$NOP_TAG_DEFAULT}"

log()  { printf '%s\n' "$*" >&2; }
warn() { printf 'WARN: %s\n' "$*" >&2; }
err()  { printf 'ERROR: %s\n' "$*" >&2; exit 1; }

resolve_tag() {
  local input="${1:-}"
  [ -z "$input" ] && { echo ""; return; }

  # exact
  if git rev-parse -q --verify "refs/tags/$input" >/dev/null; then
    echo "$input"; return
  fi
  # release- prefix
  if [[ "$input" != release-* ]] && git rev-parse -q --verify "refs/tags/release-$input" >/dev/null; then
    echo "release-$input"; return
  fi
  # v prefix
  if git rev-parse -q --verify "refs/tags/v$input" >/dev/null; then
    echo "v$input"; return
  fi
  # fuzzy last resort
  git tag -l "*$input*" | head -n1
}

normalize_release_branch_name() {
  local tag_or_version="${1:-}"
  [ -z "$tag_or_version" ] && { echo ""; return; }
  local t="$tag_or_version"
  t="${t#release-}"
  t="${t#v}"
  echo "release-$t"
}

ensure_upstream() {
  if ! git remote get-url upstream >/dev/null 2>&1; then
    git remote add upstream "$UPSTREAM" || true
  fi
  git fetch --tags --prune
}

switch_to_tag_branch() {
  local tag_input="${1:-}"
  [ -z "$tag_input" ] && return 0

  local tag
  tag="$(resolve_tag "$tag_input")"
  if [ -z "$tag" ]; then
    warn "Could not find a tag matching '$tag_input'; staying on current branch."
    return 0
  fi

  local br
  br="$(normalize_release_branch_name "$tag")"
  if git show-ref --verify --quiet "refs/heads/$br"; then
    git checkout "$br"
  else
    git checkout -b "$br" "tags/$tag"
    git push -u origin "$br" || true
  fi
}

add_or_ensure_submodule() {
  local remote="$1"
  if [ -d "$THEME_PATH/.git" ]; then
    git submodule sync -- "$THEME_PATH"
  else
    mkdir -p "$(dirname "$THEME_PATH")"
    if ! git submodule add "$remote" "$THEME_PATH"; then
      return 1
    fi
    git config -f .gitmodules "submodule.$THEME_PATH.branch" "$THEME_BRANCH"
    git add .gitmodules "$THEME_PATH"
    git commit -m "Add theme submodule at $THEME_PATH" || true
    git push || true
  fi
}

init_submodules_with_fallback() {
  local out
  if ! out=$(git submodule update --init --recursive 2>&1); then
    if echo "$out" | grep -q "Permission denied (publickey)"; then
      warn "SSH auth failed for submodule. Falling back to HTTPS."
      git config -f .gitmodules "submodule.$THEME_PATH.url" "$THEME_REMOTE_HTTPS"
      git submodule sync -- "$THEME_PATH"
      git submodule update --init --recursive
    else
      printf '%s\n' "$out" >&2
      return 1
    fi
  fi
}

# --- Main -------------------------------------------------------------

# 0) Use current repo if inside one; otherwise clone parent and cd into it
if git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  log "Using existing repo: $(git rev-parse --show-toplevel)"
else
  REPO_DIR="$(basename "$PARENT_REMOTE" .git)"
  if [ ! -d "$REPO_DIR/.git" ]; then
    log "Cloning parent repo into $REPO_DIR ..."
    git clone "$PARENT_REMOTE" "$REPO_DIR"
  fi
  cd "$REPO_DIR"
fi

# Windows long path is harmless elsewhere
git config core.longpaths true || true

# 1) Upstream & optional tag → release branch
ensure_upstream
