param(
    [ValidateSet('up','down')]
    [string]$Command = 'up',
    [switch]$Detach
)

$ErrorActionPreference = 'Stop'

function Write-Info($msg) { Write-Host "[info] $msg" -ForegroundColor Cyan }
function Write-Warn($msg) { Write-Host "[warn] $msg" -ForegroundColor Yellow }
function Write-Err($msg) { Write-Host "[error] $msg" -ForegroundColor Red }

function Require-Docker {
    try { docker version --format '{{.Server.Os}}' | Out-Null } catch {
        Write-Err "Docker Desktop is not running. Start Docker Desktop and try again."; exit 1
    }
    $serverOs = docker version --format '{{.Server.Os}}' 2>$null
    if (-not $serverOs) { Write-Err "Cannot communicate with Docker engine."; exit 1 }
    if ($serverOs.Trim().ToLower() -ne 'linux') {
        Write-Warn "Docker engine reports OS='$serverOs'. Switch to Linux containers for best compatibility."
    }
}

function Get-ComposeFilePath {
    $root = Split-Path -Parent $PSScriptRoot
    $file = Join-Path $root 'docker-compose.nop-mysql.yml'
    if (-not (Test-Path $file)) { throw "Compose file not found: $file" }
    return $file
}

function Get-Health($name) {
    try {
        $status = docker inspect -f '{{if .State.Health}}{{.State.Health.Status}}{{else}}{{.State.Status}}{{end}}' $name 2>$null
        if (-not $status) { return 'unknown' }
        return $status.Trim()
    } catch { return 'unknown' }
}

function Print-ConnectionStrings {
    Write-Host "`nSuggested nopCommerce settings (set DataProvider=MySql):" -ForegroundColor Green
    Write-Host "ConnectionString (user): Server=localhost;Port=3306;Database=nopcommerce;Uid=nop;Pwd=nop123;SslMode=None;TreatTinyAsBoolean=true;"
    Write-Host "ConnectionString (root): Server=localhost;Port=3306;Database=nopcommerce;Uid=root;Pwd=root123;SslMode=None;TreatTinyAsBoolean=true;"
}

Require-Docker

$compose = Get-ComposeFilePath

switch ($Command) {
    'up' {
        Write-Info "Pulling database image (mariadb:11.8)..."
        docker pull mariadb:11.8 | Write-Output

        $args = @('-f', $compose, 'up')
        if ($Detach) { $args += '-d' }
        Write-Info "Starting MySQL container (nop-mysql)..."
        docker compose @args | Write-Output

        Write-Info "Waiting for container health (nop-mysql)..."
        $attempts = 0
        do {
            Start-Sleep -Seconds 2
            $health = Get-Health 'nop-mysql'
            $attempts++
            Write-Host "." -NoNewline
        } while ($health -ne 'healthy' -and $attempts -lt 60)
        Write-Host ""

        if ($health -eq 'healthy') {
            Write-Info "Container is healthy."
            Print-ConnectionStrings
        } else {
            Write-Warn "Container health is '$health'. Printing connection strings anyway."
            Print-ConnectionStrings
        }
    }
    'down' {
        Write-Info "Stopping and removing MySQL container and volume..."
        docker compose -f $compose down -v | Write-Output
    }
}
