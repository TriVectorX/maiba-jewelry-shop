#!/usr/bin/env bash
set -euo pipefail

COMMAND="${1:-up}"
DETACH_FLAG=""
if [[ "${2:-}" == "-d" || "${2:-}" == "--detach" ]]; then
  DETACH_FLAG="-d"
fi

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
ROOT_DIR="${SCRIPT_DIR}/.."
COMPOSE_FILE="${ROOT_DIR}/docker-compose.nop-mysql.yml"

info() { echo -e "\033[0;36m[info]\033[0m $*"; }
warn() { echo -e "\033[0;33m[warn]\033[0m $*"; }
err()  { echo -e "\033[0;31m[error]\033[0m $*"; }

print_connection_strings() {
  echo
  echo -e "\033[0;32mSuggested nopCommerce settings (set DataProvider=MySql):\033[0m"
  echo "ConnectionString (user): Server=localhost;Port=3306;Database=nopcommerce;Uid=nop;Pwd=nop123;SslMode=None;TreatTinyAsBoolean=true;"
  echo "ConnectionString (root): Server=localhost;Port=3306;Database=nopcommerce;Uid=root;Pwd=root123;SslMode=None;TreatTinyAsBoolean=true;"
}

case "$COMMAND" in
  up)
    info "Starting MySQL container (nop-mysql)..."
    docker compose -f "$COMPOSE_FILE" up ${DETACH_FLAG} || true

    info "Waiting for container health (nop-mysql)..."
    for i in {1..60}; do
      STATUS=$(docker inspect -f '{{if .State.Health}}{{.State.Health.Status}}{{else}}{{.State.Status}}{{end}}' nop-mysql 2>/dev/null || echo "unknown")
      if [[ "$STATUS" == "healthy" ]]; then
        echo
        info "Container is healthy."
        print_connection_strings
        exit 0
      fi
      printf "."
      sleep 2
    done
    echo
    warn "Container not healthy after waiting."
    print_connection_strings
    ;;
  down)
    info "Stopping and removing MySQL container and volume..."
    docker compose -f "$COMPOSE_FILE" down -v || true
    ;;
  *)
    err "Unknown command: $COMMAND (use 'up' or 'down')"
    exit 1
    ;;
fi
