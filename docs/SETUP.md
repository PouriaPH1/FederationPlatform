# Setup Guide - Federation Platform

## Prerequisites

### Required Software
1. **.NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

2. **SQL Server 2019 or later**
   - SQL Server Express (free): https://www.microsoft.com/sql-server/sql-server-downloads
   - Or SQL Server Developer Edition
   - Or Azure SQL Database

3. **IDE (Choose one)**
   - Visual Studio 2022 (Community/Professional/Enterprise)
   - JetBrains Rider
   - Visual Studio Code with C# extension

### Optional Tools
- SQL Server Management Studio (SSMS)
- Azure Data Studio
- Git for version control

## Installation Steps

### 1. Clone or Extract the Project
```bash
cd /path/to/FederationPlatform
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Configure Database Connection

Edit `src/FederationPlatform.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=FederationPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Build the Solution
```bash
dotnet build
```

### 5. Apply Database Migrations (After Phase 4 is complete)
```bash
cd src/FederationPlatform.Web
dotnet ef database update
```

### 6. Run the Application
```bash
cd src/FederationPlatform.Web
dotnet run
```

The application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Common Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project src/FederationPlatform.Web
```

### Test
```bash
dotnet test
```

### Create Migration
```bash
dotnet ef migrations add MigrationName --project src/FederationPlatform.Infrastructure --startup-project src/FederationPlatform.Web
```

### Update Database
```bash
dotnet ef database update --project src/FederationPlatform.Infrastructure --startup-project src/FederationPlatform.Web
```

---

**Last Updated:** April 25, 2025
**Version:** 1.0
