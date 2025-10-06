param(
    [switch]$Watch,
    [string]$Configuration = 'Debug',
    [string]$Environment = 'Development'
)

$ErrorActionPreference = 'Stop'

function Write-Info($msg) { Write-Host "[info] $msg" -ForegroundColor Cyan }
function Write-Err($msg) { Write-Host "[error] $msg" -ForegroundColor Red }

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'src/NopCommerce.sln'
$webProj = Join-Path $root 'src/Presentation/Nop.Web'

if (-not (Test-Path $solution)) { throw "Solution not found: $solution" }
if (-not (Test-Path $webProj)) { throw "Web project not found: $webProj" }

Write-Info "Restoring solution..."
dotnet restore $solution

Write-Info "Building solution ($Configuration)..."
dotnet build $solution -c $Configuration --no-restore

$env:ASPNETCORE_ENVIRONMENT = $Environment
Write-Info "Running Nop.Web (ASPNETCORE_ENVIRONMENT=$Environment)..."

if ($Watch) {
    dotnet watch --project $webProj run --no-build --urls "http://localhost:5000"
} else {
    dotnet run --project $webProj --no-build --urls "http://localhost:5000"
}
