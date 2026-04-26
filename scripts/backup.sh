#!/bin/bash

# Federation Platform Backup Script
# Usage: ./backup.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
BACKUP_DIR="$PROJECT_ROOT/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "=========================================="
echo "Federation Platform Backup"
echo "Timestamp: $TIMESTAMP"
echo "=========================================="

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Load environment variables
if [ -f "$PROJECT_ROOT/.env" ]; then
    export $(cat "$PROJECT_ROOT/.env" | grep -v '^#' | xargs)
fi

# Backup database
echo "Backing up database..."
DB_BACKUP_FILE="$BACKUP_DIR/database_$TIMESTAMP.bak"

docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "${DB_PASSWORD:-YourStrong@Passw0rd}" \
    -Q "BACKUP DATABASE [FederationPlatformDb] TO DISK = N'/var/opt/mssql/backup_$TIMESTAMP.bak' WITH NOFORMAT, NOINIT, NAME = 'Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

docker cp "federation-sqlserver:/var/opt/mssql/backup_$TIMESTAMP.bak" "$DB_BACKUP_FILE"
echo "Database backup saved to: $DB_BACKUP_FILE"

# Backup uploaded files
echo "Backing up uploaded files..."
UPLOADS_BACKUP_FILE="$BACKUP_DIR/uploads_$TIMESTAMP.tar.gz"

docker run --rm \
    --volumes-from federation-web \
    -v "$BACKUP_DIR:/backup" \
    alpine tar czf "/backup/uploads_$TIMESTAMP.tar.gz" -C /app/wwwroot uploads

echo "Uploads backup saved to: $UPLOADS_BACKUP_FILE"

# Backup logs
echo "Backing up logs..."
LOGS_BACKUP_FILE="$BACKUP_DIR/logs_$TIMESTAMP.tar.gz"

docker run --rm \
    --volumes-from federation-web \
    -v "$BACKUP_DIR:/backup" \
    alpine tar czf "/backup/logs_$TIMESTAMP.tar.gz" -C /app Logs

echo "Logs backup saved to: $LOGS_BACKUP_FILE"

# Create backup manifest
MANIFEST_FILE="$BACKUP_DIR/manifest_$TIMESTAMP.txt"
cat > "$MANIFEST_FILE" << MANIFEST
Federation Platform Backup Manifest
====================================
Timestamp: $TIMESTAMP
Date: $(date)

Files:
- Database: database_$TIMESTAMP.bak
- Uploads: uploads_$TIMESTAMP.tar.gz
- Logs: logs_$TIMESTAMP.tar.gz

Sizes:
- Database: $(du -h "$DB_BACKUP_FILE" | cut -f1)
- Uploads: $(du -h "$UPLOADS_BACKUP_FILE" | cut -f1)
- Logs: $(du -h "$LOGS_BACKUP_FILE" | cut -f1)
MANIFEST

echo "Manifest saved to: $MANIFEST_FILE"

# Clean up old backups (keep last 7 days)
echo "Cleaning up old backups..."
find "$BACKUP_DIR" -name "database_*.bak" -mtime +7 -delete
find "$BACKUP_DIR" -name "uploads_*.tar.gz" -mtime +7 -delete
find "$BACKUP_DIR" -name "logs_*.tar.gz" -mtime +7 -delete
find "$BACKUP_DIR" -name "manifest_*.txt" -mtime +7 -delete

echo "=========================================="
echo "Backup completed successfully!"
echo "=========================================="
