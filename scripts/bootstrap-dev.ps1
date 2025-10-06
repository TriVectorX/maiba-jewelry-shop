param(
  [string]$ParentRemote     = "git@github.com:TriVectorX/maiba-jewelry-shop.git",
  [string]$ThemeRemoteSsh   = "git@github.com:TriVectorX/maiba-jewelry-shop-theme.git",
  [string]$ThemeRemoteHttps = "https://github.com/TriVectorX/maiba-jewelry-shop-theme.git",
  [string]$ThemePath        = "src/Presentation/Nop.Web/Themes/Maiba",
  [string]$ThemeBranch      = "main",
  [string]$Upstream         = "https://github.com/nopSolutions/nopCommerce.git",
  [string]$NopTag           = ""   # e.g. "4.80.9" or "release-4.80.9"; empty = stay on current branch
)

# --- Helpers ----------------------------------------------------------------

function Convert-RemoteToHttps([string]$Remote) {
  if ($Remote -match '^git@github\.com:(.+?)/(.+?)\.git$') {
    return "https://github.com/$($matches[1])/$($matches[2]).git"
  }
  return $Remote
}

function Resolve-Tag([string]$Input) {
  if (-not $Input -or $Input.Trim() -eq "") { return "" }

  git rev-parse "refs/tags/$Input" 2>$null | Out-Null
  if ($LASTEXITCODE -eq 0) { return $Input }

  if ($Input -notmatch '^release-') {
    $try = "release-$Input"
    git rev-parse "refs/tags/$try" 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { return $try }
  }

  $try = "v$Input"
  git rev-parse "refs/tags/$try" 2>$null | Out-Null
  if ($LASTEXITCODE -eq 0) { return $try }

  $tag = git tag -l "*$Input*" | Select-Object -First 1
  return ($tag | ForEach-Object { $_.Trim() })
}

function Normalize-ReleaseBranchName([string]$TagOrVersion) {
  if (-not $TagOrVersion) { return "" }
  $t = $TagOrVersion -replace '^release-','' -replace '^v',''
  return "release-$t"
}

function Ensure-Upstream {
  git remote get-url upstream 2>$null | Out-Null
  if ($LASTEXITCODE -ne 0) { git remote add upstream $Upstream | Out-Host }
  git fetch --tags --prune | Out-Host
}

function Switch-To-TagBranch([string]$tagInput) {
  if (-not $tagInput) { return }
  $resolved = Resolve-Tag $tagInput
  if (-not $resolved) { Write-Warning "Tag '$tagInput' not found; staying on current branch."; return }

  $branch = Normalize-ReleaseBranchName $resolved
  git show-ref --verify --quiet "refs/heads/$branch"
  if ($LASTEXITCODE -eq 0) {
    git checkout $branch | Out-Host
  } else {
    git checkout -b $branch "tags/$resolved" | Out-Host
    # push with SSH→HTTPS fallback for the PARENT repo
    git push -u origin $branch 2>$null | Out-Host
    if ($LASTEXITCODE -ne 0) {
      Write-Warning "Parent push via SSH failed. Switching origin to HTTPS and retrying…"
      $https = Convert-RemoteToHttps (git remote get-url origin)
      git remote set-url origin $https | Out-Host
      git push -u origin $branch | Out-Host
    }
  }
}

function Add-Or-Ensure-Submodule($remote) {
  if (Test-Path "$ThemePath/.git") {
    git submodule sync -- $ThemePath | Out-Host
  } else {
    New-Item -ItemType Directory -Force -Path (Split-Path $ThemePath) | Out-Null
    git submodule add $remote $ThemePath | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Failed to add submodule with remote $remote" }
    git config -f .gitmodules "submodule.$ThemePath.branch" $ThemeBranch | Out-Host
    git add .gitmodules $ThemePath | Out-Host
    git commit -m "Add theme submodule at $ThemePath" 2>$null | Out-Host

    # push with SSH→HTTPS fallback for the PARENT repo
    git push 2>$null | Out-Host
    if ($LASTEXITCODE -ne 0) {
      Write-Warning "Parent push via SSH failed. Switching origin to HTTPS and retrying…"
      $https = Convert-RemoteToHttps (git remote get-url origin)
      git remote set-url origin $https | Out-Host
      git push | Out-Host
    }
  }
}

function Init-Submodules-With-Fallback() {
  $out = git submodule update --init --recursive 2>&1
  if ($LASTEXITCODE -ne 0 -and $out -match "Permission denied \(publickey\)") {
    Write-Warning "SSH auth failed for submodule. Falling back to HTTPS."
    git config -f .gitmodules "submodule.$ThemePath.url" $ThemeRemoteHttps | Out-Host
    git submodule sync -- $ThemePath | Out-Host
    git submodule update --init --recursive | Out-Host
  } else {
    $out | Out-Host
  }
}

# --- Main -------------------------------------------------------------------

# 0) Ensure we are in the parent repo root (if not in a repo, clone parent)
git rev-parse --is-inside-work-tree 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
  $RepoDir = [IO.Path]::GetFileNameWithoutExtension(($ParentRemote -replace '\.git$',''))
  if (-not (Test-Path "$RepoDir/.git")) {
    Write-Host "Cloning parent repo into $RepoDir ..."
    git clone $ParentRemote $RepoDir | Out-Host
  }
  Set-Location $RepoDir
} else {
  Write-Host "Using existing repo: $(git rev-parse --show-toplevel)"
}

git config core.longpaths true | Out-Null

# 1) Upstream & optional tag → branch
Ensure-Upstream
Switch-To-TagBranch $NopTag

# 2) Ensure submodule (SSH first, HTTPS fallback)
try {
  Add-Or-Ensure-Submodule $ThemeRemoteSsh
} catch {
  Write-Warning $_.Exception.Message
  Write-Warning "Retrying submodule add via HTTPS…"
  Add-Or-Ensure-Submodule $ThemeRemoteHttps
}

# 3) Init/update submodules (SSH→HTTPS fallback)
Init-Submodules-With-Fallback

# 4) Verify theme path exists
if (-not (Test-Path $ThemePath)) {
  Write-Error "Theme path '$ThemePath' does not exist after submodule init.
Check your credentials (ssh -T git@github.com) or re-run using HTTPS remotes."
  exit 1
}

# 5) Ensure theme branch locally (do NOT force SSH; keep whatever works)
Push-Location $ThemePath
git fetch origin --prune 2>$null | Out-Host
$hasBranch = (git branch --list $ThemeBranch)
if (-not $hasBranch) {
  git checkout -B $ThemeBranch origin/$ThemeBranch 2>$null
  if ($LASTEXITCODE -ne 0) { git checkout -B $ThemeBranch | Out-Host }
} else {
  git checkout $ThemeBranch | Out-Host
  git pull --ff-only 2>$null | Out-Host
}
Pop-Location

git config diff.submodule log | Out-Null
git config status.submoduleSummary 1 | Out-Null

Write-Host "✔ Bootstrap complete. Work in $ThemePath, then bump the submodule pointer in the parent."
