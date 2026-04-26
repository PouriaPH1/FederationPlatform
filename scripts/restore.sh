#!/bin/bash

# Federation Platform Restore Script
# Usage: ./restore.sh [backup_timestamp]
# Example: ./restore.sh 20260425_120000

set -e

BACKUP_TIMESTAMP=$1
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
BACKUP_DIR="$PROJECT_ROOT/backups"

if [ -z "$BACKUP_TIMESTAMP" ]; then
    echo "Error: Backup timestamp is required"
    echo "Usage: ./restore.sh [backup_timestamp]"
    echo ""
    echo "Available backups:"
    ls -1 "$BACKUP_DIR"/database_*.bak | sed 's/.*database_\(.*\)\.bak/\1/'
    exit 1
fi

echo "=========================================="
echo "Federation Platform Restore"
echo "Timestamp: $BACKUP_TIMESTAMP"
echo "=========================================="

# Load environment variables
if [ -f "$PROJECT_ROOT/.env" ]; then
    export $(cat "$PROJECT_ROOT/.env" | grep -v '^#' | xargs)
fi

# Check if backup files exist
DB_BACKUP_FILE="$BACKUP_DIR/database_$BACKUP_TIMESTAMP.bak"
UPLOADS_BACKUP_FILE="$BACKUP_DIR/uploads_$BACKUP_TIMESTAMP.tar.gz"

if [ ! -f "$DB_BACKUP_FILE" ]; then
    echo "Error: Database backup file not found: $DB_BACKUP_FILE"
    exit 1
fi

# Confirm restore
read -p "This will overwrite the current database and files. Continue? (yes/no): " CONFIRM
if [ "$CONFIRM" != "yes" ]; then
    echo "Restore cancelled"
    exit 0
fi

# Stop application
echo "Stopping application..."
cd "$PROJECT_ROOT"
docker-compose stop web

# Restore database
echo "Restoring database..."
docker cp "$DB_BACKUP_FILE" "federation-sqlserver:/var/opt/mssql/restore.bak"

docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "${DB_PASSWORD:-YourStrong@Passw0rd}" \
    -Q "RESTORE DATABASE [FederationPlatformDb] FROM DISK = N'/var/opt/mssql/restore.bak' WITH REPLACE, STATS = 10"

echo "Database restored successfully"

# Restore uploaded files
if [ -f "$UPLOADS_BACKUP_FILE" ]; then
    echo "Restoring uploaded files..."
    docker run --rm \
        --volumes-from federation-web \
        -v "$BACKUP_DIR:/backup" \
        alpine sh -c "rm -rf /app/wwwroot/uploads/* && tar xzf /backup/uploads_$BACKUP_TIMESTAMP.tar.gz -C /app/wwwroot"
    echo "Uploads restored successfully"
fi

# Start application
echo "Starting application..."
docker-compose up -d

# Wait for health check
echo "Waiting for application to start..."
sleep 30

if curl -f http://localhost:5000/health > /dev/null 2>&1; then
    echo "=========================================="
    echo "Restore completed successfully!"
    echo "=========================================="
else
    echo "Warning: Health check failed. Please check application logs."
    docker-compose logs web
fi
