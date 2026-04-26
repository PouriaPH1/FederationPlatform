@echo off
REM Federation Platform - Local Development Quick Start (Windows)

echo ==========================================
echo Federation Platform - Local Setup
echo ==========================================
echo.

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo X Docker is not installed!
    echo Please install Docker Desktop from: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo [OK] Docker is installed

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo X Docker is not running!
    echo Please start Docker Desktop and try again
    pause
    exit /b 1
)

echo [OK] Docker is running

REM Check if .env file exists
if not exist .env (
    echo Creating .env file from example...
    copy .env.example .env
    echo [OK] .env file created
) else (
    echo [OK] .env file exists
)

echo.
echo Starting services...
echo.

REM Start Docker Compose
docker-compose -f docker-compose.dev.yml up -d

echo.
echo Waiting for services to start (30 seconds)...
timeout /t 30 /nobreak >nul

echo.
echo Running database migrations...
docker-compose -f docker-compose.dev.yml exec -T web dotnet ef database update

echo.
echo ==========================================
echo [OK] Federation Platform is running!
echo ==========================================
echo.
echo Access Points:
echo    - Web Application: http://localhost:5000
echo    - Health Check: http://localhost:5000/health
echo    - MailHog (Email): http://localhost:8025
echo.
echo Default Admin Login:
echo    - Email: admin@federation.ir
echo    - Password: Admin@123
echo.
echo View Logs:
echo    docker-compose -f docker-compose.dev.yml logs -f
echo.
echo Stop Services:
echo    docker-compose -f docker-compose.dev.yml down
echo.
echo ==========================================
echo.
pause
