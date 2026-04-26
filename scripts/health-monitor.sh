#!/bin/bash

# Federation Platform Health Monitoring Script
# Usage: ./health-monitor.sh

set -e

HEALTH_URL="${HEALTH_URL:-http://localhost:5000/health}"
CHECK_INTERVAL="${CHECK_INTERVAL:-60}"
ALERT_EMAIL="${ALERT_EMAIL:-admin@federation.ir}"
LOG_FILE="logs/health-monitor.log"

echo "=========================================="
echo "Federation Platform Health Monitor"
echo "URL: $HEALTH_URL"
echo "Interval: ${CHECK_INTERVAL}s"
echo "=========================================="

# Create log directory if it doesn't exist
mkdir -p logs

# Function to log messages
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Function to send alert
send_alert() {
    local subject="$1"
    local message="$2"
    
    # Send email alert (requires mail command)
    if command -v mail >/dev/null 2>&1; then
        echo "$message" | mail -s "$subject" "$ALERT_EMAIL"
        log_message "Alert sent: $subject"
    else
        log_message "Warning: mail command not found, cannot send alert"
    fi
}

# Function to check health
check_health() {
    local response
    local http_code
    
    response=$(curl -s -w "\n%{http_code}" "$HEALTH_URL" 2>&1)
    http_code=$(echo "$response" | tail -n1)
    
    if [ "$http_code" = "200" ]; then
        log_message "Health check passed (HTTP $http_code)"
        return 0
    else
        log_message "Health check failed (HTTP $http_code)"
        return 1
    fi
}

# Main monitoring loop
consecutive_failures=0
max_failures=3

log_message "Health monitoring started"

while true; do
    if check_health; then
        consecutive_failures=0
    else
        consecutive_failures=$((consecutive_failures + 1))
        
        if [ $consecutive_failures -eq $max_failures ]; then
            send_alert "Federation Platform Health Alert" \
                "Health check has failed $consecutive_failures consecutive times.\n\nURL: $HEALTH_URL\nTime: $(date)\n\nPlease investigate immediately."
        fi
    fi
    
    sleep "$CHECK_INTERVAL"
done
