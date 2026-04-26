#!/bin/bash

# Federation Platform - Local Development Quick Start
# This script helps you start the project locally

set -e

echo "=========================================="
echo "Federation Platform - Local Setup"
echo "=========================================="
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed!"
    echo "Please install Docker Desktop from: https://www.docker.com/products/docker-desktop"
    exit 1
fi

echo "✅ Docker is installed"

# Check if Docker is running
if ! docker info &> /dev/null; then
    echo "❌ Docker is not running!"
    echo "Please start Docker Desktop and try again"
    exit 1
fi

echo "✅ Docker is running"

# Check if .env file exists
if [ ! -f .env ]; then
    echo "📝 Creating .env file from example..."
    cp .env.example .env
    echo "✅ .env file created"
else
    echo "✅ .env file exists"
fi

echo ""
echo "Starting services..."
echo ""

# Start Docker Compose
docker-compose -f docker-compose.dev.yml up -d

echo ""
echo "⏳ Waiting for services to start (30 seconds)..."
sleep 30

echo ""
echo "🔄 Running database migrations..."
docker-compose -f docker-compose.dev.yml exec -T web dotnet ef database update || {
    echo "⚠️  Migration failed, but services are running"
    echo "You may need to run migrations manually"
}

echo ""
echo "=========================================="
echo "✅ Federation Platform is running!"
echo "=========================================="
echo ""
echo "📍 Access Points:"
echo "   - Web Application: http://localhost:5000"
echo "   - Health Check: http://localhost:5000/health"
echo "   - MailHog (Email): http://localhost:8025"
echo ""
echo "👤 Default Admin Login:"
echo "   - Email: admin@federation.ir"
echo "   - Password: Admin@123"
echo ""
echo "📊 View Logs:"
echo "   docker-compose -f docker-compose.dev.yml logs -f"
echo ""
echo "🛑 Stop Services:"
echo "   docker-compose -f docker-compose.dev.yml down"
echo ""
echo "=========================================="
