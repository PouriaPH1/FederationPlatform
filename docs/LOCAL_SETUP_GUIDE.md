# Local Setup Guide - Federation Platform
## راهنمای راه‌اندازی محلی پلتفرم فدراسیون

---

## Quick Start (Recommended)

### Option 1: Using Docker (Easiest)

#### Prerequisites
- Docker Desktop installed
- 8GB RAM available
- 20GB disk space

#### Steps

1. **Clone the repository** (if not already done)
```bash
cd /mnt/c/Users/leon/Desktop/uni_project/FederationPlatform
```

2. **Create environment file**
```bash
cp .env.example .env
```

3. **Start all services**
```bash
docker-compose -f docker-compose.dev.yml up -d
```

4. **Wait for services to start** (about 30-60 seconds)
```bash
docker-compose -f docker-compose.dev.yml logs -f
```

5. **Run database migrations**
```bash
docker-compose -f docker-compose.dev.yml exec web dotnet ef database update
```

6. **Access the application**
- **Web Application**: http://localhost:5000
- **MailHog (Email Testing)**: http://localhost:8025
- **SQL Server**: localhost:1433

7. **Default Admin Credentials** (created by seed data)
- Email: `admin@federation.ir`
- Password: `Admin@123`

---

## Option 2: Manual Setup (Without Docker)

### Prerequisites

1. **.NET 8.0 SDK**
```bash
# Check if installed
dotnet --version

# If not installed, download from:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

2. **SQL Server**
   - SQL Server 2019+ or SQL Server Express
   - Or use Docker for SQL Server only:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Dev@Passw0rd123" \
   -p 1433:1433 --name sqlserver-dev \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

3. **Visual Studio 2022 or VS Code** (optional but recommended)

### Steps

1. **Navigate to project directory**
```bash
cd /mnt/c/Users/leon/Desktop/uni_project/FederationPlatform
```

2. **Restore NuGet packages**
```bash
dotnet restore FederationPlatform.sln
```

3. **Update connection string**
Edit `src/FederationPlatform.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FederationPlatformDb_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

4. **Run database migrations**
```bash
cd src/FederationPlatform.Web
dotnet ef database update
```

5. **Run the application**
```bash
dotnet run --project src/FederationPlatform.Web/FederationPlatform.Web.csproj
```

6. **Access the application**
- Open browser: http://localhost:5000 or https://localhost:5001

---

## Detailed Setup Instructions

### Step 1: Install Prerequisites

#### Windows

**Install .NET 8.0 SDK:**
1. Download from https://dotnet.microsoft.com/download/dotnet/8.0
2. Run installer
3. Verify: `dotnet --version`

**Install Docker Desktop:**
1. Download from https://www.docker.com/products/docker-desktop
2. Install and restart
3. Verify: `docker --version`

**Install SQL Server (if not using Docker):**
1. Download SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
2. Install with default settings
3. Remember SA password

#### Linux (Ubuntu/Debian)

```bash
# Install .NET 8.0
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Install Docker
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo systemctl start docker
sudo usermod -aG docker $USER

# Restart terminal or run
newgrp docker
```

#### macOS

```bash
# Install .NET 8.0
brew install dotnet@8

# Install Docker Desktop
brew install --cask docker
```

---

### Step 2: Database Setup

#### Option A: Using Docker (Recommended)

```bash
# Start SQL Server container
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Dev@Passw0rd123" \
  -p 1433:1433 \
  --name sqlserver-dev \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Verify it's running
docker ps | grep sqlserver-dev
```

#### Option B: Using Local SQL Server

1. Open SQL Server Management Studio (SSMS)
2. Connect to your local instance
3. Create a new database: `FederationPlatformDb_Dev`
4. Note your connection details

---

### Step 3: Configure Application

1. **Copy development settings** (if not exists)
```bash
cd src/FederationPlatform.Web
cp appsettings.json appsettings.Development.json
```

2. **Edit appsettings.Development.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FederationPlatformDb_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "EmailSettings": {
    "SmtpServer": "localhost",
    "SmtpPort": 1025,
    "SenderEmail": "noreply@federation.ir",
    "SenderName": "Federation Platform (Dev)",
    "EnableSsl": false
  },
  "FileUploadSettings": {
    "MaxFileSizeInMB": 10,
    "AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.doc,.docx",
    "UploadPath": "wwwroot/uploads"
  }
}
```

---

### Step 4: Run Database Migrations

```bash
# Navigate to Web project
cd src/FederationPlatform.Web

# Install EF Core tools (if not installed)
dotnet tool install --global dotnet-ef

# Run migrations
dotnet ef database update

# You should see output like:
# Applying migration '20260425_InitialCreate'
# Done.
```

---

### Step 5: Run the Application

#### Using Command Line

```bash
# From project root
dotnet run --project src/FederationPlatform.Web/FederationPlatform.Web.csproj

# Or from Web project directory
cd src/FederationPlatform.Web
dotnet run
```

#### Using Visual Studio

1. Open `FederationPlatform.sln`
2. Set `FederationPlatform.Web` as startup project
3. Press F5 or click "Run"

#### Using VS Code

1. Open project folder
2. Install C# extension
3. Press F5
4. Select ".NET Core Launch (web)"

---

### Step 6: Verify Installation

1. **Check application is running**
```bash
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": {...}
}
```

2. **Open in browser**
- Navigate to: http://localhost:5000
- You should see the login page

3. **Login with default admin**
- Email: `admin@federation.ir`
- Password: `Admin@123`

---

## Troubleshooting

### Issue: Port 5000 already in use

**Solution:**
```bash
# Find process using port 5000
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000

# Kill the process or change port in launchSettings.json
```

### Issue: Cannot connect to SQL Server

**Solution:**
```bash
# Check if SQL Server is running
docker ps | grep sqlserver

# Check connection string
# Verify password and server name

# Test connection
docker exec -it sqlserver-dev /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P Dev@Passw0rd123 -Q "SELECT 1"
```

### Issue: Migration fails

**Solution:**
```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update

# Or manually in SQL Server
DROP DATABASE FederationPlatformDb_Dev;
```

### Issue: NuGet restore fails

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore FederationPlatform.sln
```

### Issue: Build errors

**Solution:**
```bash
# Clean solution
dotnet clean FederationPlatform.sln

# Rebuild
dotnet build FederationPlatform.sln
```

---

## Development Workflow

### Running with Hot Reload

```bash
cd src/FederationPlatform.Web
dotnet watch run
```

Now any code changes will automatically reload the application.

### Running Tests

```bash
# Run all tests
dotnet test FederationPlatform.sln

# Run unit tests only
dotnet test tests/FederationPlatform.UnitTests/FederationPlatform.UnitTests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Commands

```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Drop database
dotnet ef database drop
```

### Viewing Logs

```bash
# Logs are in
tail -f src/FederationPlatform.Web/Logs/federation-platform-*.log

# Or check console output
```

---

## Email Testing (Development)

### Using MailHog (Docker)

```bash
# Start MailHog
docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog

# Access web interface
# http://localhost:8025
```

All emails sent by the application will appear in MailHog.

---

## Stopping the Application

### Docker

```bash
# Stop all services
docker-compose -f docker-compose.dev.yml down

# Stop and remove volumes (clean slate)
docker-compose -f docker-compose.dev.yml down -v
```

### Manual

- Press `Ctrl+C` in the terminal where the app is running

---

## Quick Reference

### URLs
- **Application**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Health Check**: http://localhost:5000/health
- **MailHog**: http://localhost:8025

### Default Credentials
- **Admin**: admin@federation.ir / Admin@123
- **Representative**: rep@university1.ir / Rep@123
- **User**: user@university1.ir / User@123

### Database
- **Server**: localhost,1433
- **Database**: FederationPlatformDb_Dev
- **User**: sa
- **Password**: Dev@Passw0rd123

### Useful Commands
```bash
# Check .NET version
dotnet --version

# Check Docker
docker --version

# List running containers
docker ps

# View application logs
docker-compose -f docker-compose.dev.yml logs -f web

# Restart application
docker-compose -f docker-compose.dev.yml restart web
```

---

## Next Steps

1. ✅ Application running locally
2. 📖 Read the [User Guide](USER_GUIDE.md)
3. 🔧 Check [Development Guide](DEVELOPMENT_GUIDE.md)
4. 🧪 Run tests: `dotnet test`
5. 🚀 Start developing!

---

## Getting Help

- Check logs in `src/FederationPlatform.Web/Logs/`
- Review error messages in console
- Check database connection
- Verify all prerequisites installed
- Contact: support@federation.ir

---

**Last Updated**: April 25, 2026
