param(
  [Parameter(Mandatory=$true)][string]$NopTag,
  [string]$ThemeRemote = "git@github.com:TriVectorX/maiba-jewelry-shop-theme.git",
  [string]$ThemePath   = "src/Presentation/Nop.Web/Themes/Maiba",
  [string]$ThemeBranch = "main",
  [string]$Upstream    = "https://github.com/nopSolutions/nopCommerce.git"
)

Write-Host "Upgrading to nopCommerce tag: $NopTag"
Write-Host "Theme remote: $ThemeRemote"
Write-Host "Theme path  : $ThemePath"
Write-Host "Theme branch: $ThemeBranch"
Write-Host ""

# 1) Ensure we're in a git repo
git rev-parse --is-inside-work-tree 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
  throw "Not inside a Git repository."
}

# 2) Ensure upstream remote
git remote get-url upstream 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
  git remote add upstream $Upstream | Out-Host
}

# 3) Fetch upstream tags
git fetch upstream --tags --prune | Out-Host

# 4) Create new release branch from tag
$branch = "release-$NopTag"
git show-ref --verify --quiet "refs/heads/$branch"
if ($LASTEXITCODE -eq 0) {
  Write-Host "Branch $branch already exists. Checking out..."
  git checkout $branch | Out-Host
} else {
  Write-Host "Creating branch $branch from tags/$NopTag ..."
  git checkout -b $branch "tags/$NopTag" | Out-Host
  git push -u origin $branch | Out-Host
}

# 5) Ensure submodule exists & points to ThemeRemote
if (Test-Path "$ThemePath/.git") {
  git submodule sync -- $ThemePath | Out-Host
} else {
  Write-Host "Adding theme submodule (upgrade flow)..."
  New-Item -ItemType Directory -Force -Path (Split-Path $ThemePath) | Out-Null
  git submodule add $ThemeRemote $ThemePath | Out-Host
  git config -f .gitmodules "submodule.$ThemePath.branch" $ThemeBranch | Out-Host
  git add .gitmodules $ThemePath | Out-Host
  git commit -m "Add theme submodule for $branch" | Out-Host
  git push | Out-Host
}

# 6) Init/Update submodules
git submodule update --init --recursive | Out-Host

# 7) Checkout theme branch
Push-Location $ThemePath
git remote set-url origin $ThemeRemote 2>$null
git fetch origin --prune | Out-Host
$hasBranch = (git branch --list $ThemeBranch)
if (-not $hasBranch) {
  git checkout -B $ThemeBranch origin/$ThemeBranch 2>$null
  if ($LASTEXITCODE -ne 0) { git checkout -B $ThemeBranch | Out-Host }
} else {
  git checkout $ThemeBranch | Out-Host
  git pull --ff-only | Out-Host
}
Pop-Location

git config diff.submodule log | Out-Null
git config status.submoduleSummary 1 | Out-Null

Write-Host ""
Write-Host "✔ Upgrade branch ready: $branch"
Write-Host "Adapt/test your theme against $NopTag, then commit theme changes and bump the submodule pointer in the parent repo."
