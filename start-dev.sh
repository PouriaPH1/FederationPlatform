#!/bin/bash

# Federation Platform - Development Mode with Hot Reload

echo "=========================================="
echo "Federation Platform - Development Mode"
echo "=========================================="
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed!"
    echo "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

echo "✅ .NET SDK is installed"

# Start SQL Server with Docker
echo ""
echo "Starting SQL Server..."
docker run -d --name sqlserver-dev \
    -e "ACCEPT_EULA=Y" \
    -e "SA_PASSWORD=Dev@Passw0rd123" \
    -p 1433:1433 \
    mcr.microsoft.com/mssql/server:2022-latest 2>/dev/null || {
    echo "SQL Server container already exists, starting it..."
    docker start sqlserver-dev >/dev/null 2>&1
}

echo "✅ SQL Server is running"

# Wait for SQL Server
echo "Waiting for SQL Server to be ready..."
sleep 10

# Run migrations
echo ""
echo "Running database migrations..."
cd src/FederationPlatform.Web
dotnet ef database update

# Start application with hot reload
echo ""
echo "=========================================="
echo "Starting application with HOT RELOAD..."
echo "=========================================="
echo ""
echo "Any changes you make will be automatically applied!"
echo ""
echo "📍 Access Points:"
echo "   - Web Application: http://localhost:5000"
echo "   - HTTPS: https://localhost:5001"
echo ""
echo "Press Ctrl+C to stop"
echo "=========================================="
echo ""

dotnet watch run
