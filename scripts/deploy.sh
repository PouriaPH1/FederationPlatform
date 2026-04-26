#!/bin/bash

# Federation Platform Deployment Script
# Usage: ./deploy.sh [environment]
# Example: ./deploy.sh production

set -e

ENVIRONMENT=${1:-staging}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "=========================================="
echo "Federation Platform Deployment"
echo "Environment: $ENVIRONMENT"
echo "=========================================="

# Load environment variables
if [ -f "$PROJECT_ROOT/.env.$ENVIRONMENT" ]; then
    echo "Loading environment variables from .env.$ENVIRONMENT"
    export $(cat "$PROJECT_ROOT/.env.$ENVIRONMENT" | grep -v '^#' | xargs)
else
    echo "Warning: .env.$ENVIRONMENT file not found"
fi

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "Checking prerequisites..."
if ! command_exists docker; then
    echo "Error: Docker is not installed"
    exit 1
fi

if ! command_exists docker-compose; then
    echo "Error: Docker Compose is not installed"
    exit 1
fi

# Pull latest images
echo "Pulling latest Docker images..."
cd "$PROJECT_ROOT"
docker-compose pull

# Stop existing containers
echo "Stopping existing containers..."
docker-compose down

# Backup database (production only)
if [ "$ENVIRONMENT" = "production" ]; then
    echo "Creating database backup..."
    BACKUP_DIR="$PROJECT_ROOT/backups"
    mkdir -p "$BACKUP_DIR"
    BACKUP_FILE="$BACKUP_DIR/db_backup_$(date +%Y%m%d_%H%M%S).bak"
    
    docker-compose up -d sqlserver
    sleep 10
    
    docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P "$DB_PASSWORD" \
        -Q "BACKUP DATABASE [FederationPlatformDb] TO DISK = N'/var/opt/mssql/backup.bak' WITH NOFORMAT, NOINIT, NAME = 'Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
    
    docker cp federation-sqlserver:/var/opt/mssql/backup.bak "$BACKUP_FILE"
    echo "Database backup created: $BACKUP_FILE"
    
    docker-compose down
fi

# Start containers
echo "Starting containers..."
docker-compose up -d

# Wait for services to be healthy
echo "Waiting for services to be healthy..."
sleep 30

# Run database migrations
echo "Running database migrations..."
docker-compose exec -T web dotnet ef database update --project /app/FederationPlatform.Infrastructure.dll

# Health check
echo "Performing health check..."
MAX_RETRIES=10
RETRY_COUNT=0

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -f http://localhost:5000/health > /dev/null 2>&1; then
        echo "Health check passed!"
        break
    else
        RETRY_COUNT=$((RETRY_COUNT + 1))
        echo "Health check failed. Retry $RETRY_COUNT/$MAX_RETRIES..."
        sleep 5
    fi
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "Error: Health check failed after $MAX_RETRIES attempts"
    docker-compose logs web
    exit 1
fi

# Show running containers
echo "Running containers:"
docker-compose ps

echo "=========================================="
echo "Deployment completed successfully!"
echo "Application URL: http://localhost:5000"
echo "=========================================="
