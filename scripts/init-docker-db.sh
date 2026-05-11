#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
CONTAINER="${CONTAINER_NAME:-realestate-sql}"
SA_PASSWORD="${MSSQL_SA_PASSWORD:-RealEstate_local_1}"
INIT_LOCAL="$ROOT/docker/init/01-realestate.sql"
INIT_REMOTE="/tmp/01-realestate.sql"

sqlcmd_bin() {
  if docker exec "$CONTAINER" test -x /opt/mssql-tools18/bin/sqlcmd 2>/dev/null; then
    echo /opt/mssql-tools18/bin/sqlcmd
  elif docker exec "$CONTAINER" test -x /opt/mssql-tools/bin/sqlcmd 2>/dev/null; then
    echo /opt/mssql-tools/bin/sqlcmd
  else
    echo "sqlcmd not found inside container." >&2
    exit 1
  fi
}

SQLCMD="$(sqlcmd_bin)"

echo "Waiting for SQL Server in $CONTAINER ..."
for i in $(seq 1 60); do
  if docker exec "$CONTAINER" "$SQLCMD" -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &>/dev/null; then
    break
  fi
  sleep 2
  if [[ "$i" == 60 ]]; then
    echo "Timed out waiting for SQL Server." >&2
    exit 1
  fi
done

echo "Copying schema into container ..."
docker cp "$INIT_LOCAL" "$CONTAINER:$INIT_REMOTE"

echo "Applying schema ..."
docker exec "$CONTAINER" "$SQLCMD" -S localhost -U sa -P "$SA_PASSWORD" -C -d master -i "$INIT_REMOTE"

docker exec "$CONTAINER" rm -f "$INIT_REMOTE" 2>/dev/null || true
echo "Done."
