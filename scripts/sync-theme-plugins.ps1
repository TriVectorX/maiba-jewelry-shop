#!/usr/bin/env pwsh
# Sync Maiba Theme Plugins to Build Folder
# Run this after pulling the Maiba theme submodule or when you update plugin code

param(
    [switch]$Reverse,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# Paths
$themePluginsPath = Join-Path $PSScriptRoot "..\src\Presentation\Nop.Web\Themes\Maiba\Plugins"
$buildPluginsPath = Join-Path $PSScriptRoot "..\src\Plugins"

# Colors
function Write-Info($message) {
    Write-Host "ℹ️  $message" -ForegroundColor Cyan
}

function Write-Success($message) {
    Write-Host "✅ $message" -ForegroundColor Green
}

function Write-Warning($message) {
    Write-Host "⚠️  $message" -ForegroundColor Yellow
}

function Write-Error($message) {
    Write-Host "❌ $message" -ForegroundColor Red
}

# Check if directories exist
if (-not (Test-Path $themePluginsPath)) {
    Write-Error "Theme plugins folder not found: $themePluginsPath"
    exit 1
}

if (-not (Test-Path $buildPluginsPath)) {
    Write-Error "Build plugins folder not found: $buildPluginsPath"
    exit 1
}

# Get all plugin directories in theme folder
$themePlugins = Get-ChildItem -Path $themePluginsPath -Directory | Where-Object { $_.Name -like "Nop.Plugin.*" }

if ($themePlugins.Count -eq 0) {
    Write-Warning "No plugins found in theme folder"
    exit 0
}

Write-Info "Found $($themePlugins.Count) plugin(s) in Maiba theme"
Write-Host ""

foreach ($plugin in $themePlugins) {
    $pluginName = $plugin.Name
    $sourcePath = $plugin.FullName
    $targetPath = Join-Path $buildPluginsPath $pluginName
    
    if ($Reverse) {
        # Reverse sync: Build → Theme
        $sourcePath = $targetPath
        $targetPath = $plugin.FullName
        $direction = "Build → Theme"
    } else {
        # Normal sync: Theme → Build
        $direction = "Theme → Build"
    }
    
    Write-Info "Processing: $pluginName ($direction)"
    
    if ($DryRun) {
        Write-Host "  [DRY RUN] Would copy from:" -ForegroundColor Yellow
        Write-Host "    $sourcePath" -ForegroundColor Gray
        Write-Host "  [DRY RUN] To:" -ForegroundColor Yellow
        Write-Host "    $targetPath" -ForegroundColor Gray
    } else {
        try {
            # Create target directory if it doesn't exist
            if (-not (Test-Path $targetPath)) {
                New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
                Write-Host "  Created directory: $targetPath" -ForegroundColor Gray
            }
            
            # Copy all files and folders
            Copy-Item -Path "$sourcePath\*" -Destination $targetPath -Recurse -Force
            
            Write-Success "  Synced: $pluginName"
        } catch {
            Write-Error "  Failed to sync $pluginName : $_"
        }
    }
    
    Write-Host ""
}

if ($DryRun) {
    Write-Warning "DRY RUN completed. No files were actually copied."
    Write-Host "Run without -DryRun to perform the actual sync."
} else {
    Write-Success "Plugin sync completed!"
    
    if (-not $Reverse) {
        Write-Host ""
        Write-Info "Next steps:"
        Write-Host "  1. Build the solution: dotnet build" -ForegroundColor Gray
        Write-Host "  2. Run NopCommerce: .\scripts\run-nop.ps1" -ForegroundColor Gray
    }
}

