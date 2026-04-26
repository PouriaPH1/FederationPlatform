#!/bin/bash

# Federation Platform Performance Monitoring Script
# Usage: ./performance-monitor.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
LOG_FILE="$PROJECT_ROOT/logs/performance-monitor.log"

echo "=========================================="
echo "Federation Platform Performance Monitor"
echo "=========================================="

# Create log directory
mkdir -p "$PROJECT_ROOT/logs"

# Function to log messages
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Function to get container stats
get_container_stats() {
    local container=$1
    docker stats "$container" --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"
}

# Function to check disk space
check_disk_space() {
    log_message "=== Disk Space ==="
    df -h | grep -E "^/dev/" | tee -a "$LOG_FILE"
}

# Function to check database size
check_database_size() {
    log_message "=== Database Size ==="
    docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P "${DB_PASSWORD:-YourStrong@Passw0rd}" \
        -Q "SELECT name, size * 8 / 1024 AS SizeMB FROM sys.master_files WHERE database_id = DB_ID('FederationPlatformDb')" \
        2>/dev/null | tee -a "$LOG_FILE" || log_message "Failed to get database size"
}

# Function to check active connections
check_active_connections() {
    log_message "=== Active Database Connections ==="
    docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P "${DB_PASSWORD:-YourStrong@Passw0rd}" \
        -Q "SELECT COUNT(*) AS ActiveConnections FROM sys.dm_exec_sessions WHERE is_user_process = 1" \
        2>/dev/null | tee -a "$LOG_FILE" || log_message "Failed to get active connections"
}

# Function to check response time
check_response_time() {
    log_message "=== Response Time ==="
    local start_time=$(date +%s%N)
    curl -s -o /dev/null http://localhost:5000/health
    local end_time=$(date +%s%N)
    local duration=$(( (end_time - start_time) / 1000000 ))
    log_message "Health endpoint response time: ${duration}ms"
}

# Main monitoring
log_message "=========================================="
log_message "Performance Monitoring Report"
log_message "=========================================="

# Container stats
log_message "=== Container Statistics ==="
get_container_stats "federation-web" | tee -a "$LOG_FILE"
get_container_stats "federation-sqlserver" | tee -a "$LOG_FILE"
get_container_stats "federation-redis" | tee -a "$LOG_FILE"

echo "" | tee -a "$LOG_FILE"

# Disk space
check_disk_space

echo "" | tee -a "$LOG_FILE"

# Database metrics
check_database_size

echo "" | tee -a "$LOG_FILE"

check_active_connections

echo "" | tee -a "$LOG_FILE"

# Response time
check_response_time

log_message "=========================================="
log_message "Performance monitoring completed"
log_message "=========================================="
