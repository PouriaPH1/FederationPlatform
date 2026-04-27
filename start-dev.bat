@echo off
REM Federation Platform - Development Mode with Hot Reload

echo ==========================================
echo Federation Platform - Development Mode
echo ==========================================
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo X .NET SDK is not installed!
    echo Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo [OK] .NET SDK is installed

REM Start SQL Server with Docker
echo.
echo Starting SQL Server...
docker run -d --name sqlserver-dev -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Dev@Passw0rd123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest 2>nul
if errorlevel 1 (
    echo SQL Server container already exists, starting it...
    docker start sqlserver-dev >nul 2>&1
)

echo [OK] SQL Server is running

REM Wait for SQL Server
echo Waiting for SQL Server to be ready...
timeout /t 10 /nobreak >nul

REM Database migrations are handled by DbInitializer in the application startup
REM cd src\FederationPlatform.Web
REM dotnet ef database update

echo ==========================================
echo Starting application with HOT RELOAD...
echo ==========================================
echo.
echo Any changes you make will be automatically applied!
echo.
echo Access Points:
echo    - Web Application: http://localhost:5000
echo    - HTTPS: https://localhost:5001
echo.
echo Press Ctrl+C to stop
echo ==========================================
echo.

cd src\FederationPlatform.Web
dotnet watch run
