#!/usr/bin/env bash
set -euo pipefail

WATCH="false"
CONFIGURATION="Debug"
ENVIRONMENT="Development"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --watch)
      WATCH="true"; shift ;;
    -c|--configuration)
      CONFIGURATION="${2:-Debug}"; shift 2 ;;
    -e|--env|--environment)
      ENVIRONMENT="${2:-Development}"; shift 2 ;;
    *)
      echo "Unknown argument: $1"; exit 1 ;;
  esac
done

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
ROOT_DIR="${SCRIPT_DIR}/.."
SOLUTION="${ROOT_DIR}/src/NopCommerce.sln"
WEB_PROJ="${ROOT_DIR}/src/Presentation/Nop.Web"

if [[ ! -f "$SOLUTION" ]]; then echo "Solution not found: $SOLUTION"; exit 1; fi
if [[ ! -d "$WEB_PROJ" ]]; then echo "Web project not found: $WEB_PROJ"; exit 1; fi

echo "[info] Restoring solution..."
dotnet restore "$SOLUTION"

echo "[info] Building solution ($CONFIGURATION)..."
dotnet build "$SOLUTION" -c "$CONFIGURATION" --no-restore

export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"
echo "[info] Running Nop.Web (ASPNETCORE_ENVIRONMENT=$ENVIRONMENT)..."

if [[ "$WATCH" == "true" ]]; then
  dotnet watch --project "$WEB_PROJ" run --no-build --urls "http://localhost:5000"
else
  dotnet run --project "$WEB_PROJ" --no-build --urls "http://localhost:5000"
fi
