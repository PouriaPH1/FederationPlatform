# Federation Platform - Student Committee Management System

## پلتفرم یکپارچه مدیریت فعالیت‌های کمیته دانشجویی فدراسیون اقتصاد سلامت

### Overview
This is a comprehensive web platform for managing student activities across 25 medical universities in Iran, built with ASP.NET Core 8.0 and Clean Architecture.

### Architecture
- **Clean Architecture** with clear separation of concerns
- **Domain Layer**: Core business entities and logic
- **Application Layer**: Business logic, DTOs, and interfaces
- **Infrastructure Layer**: Data access, EF Core, Identity
- **Presentation Layer**: ASP.NET Core MVC Web Application

### Technology Stack
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- ASP.NET Core Identity
- AutoMapper
- FluentValidation
- SQL Server
- Bootstrap 5
- Chart.js

### Project Structure
```
FederationPlatform/
├── src/
│   ├── FederationPlatform.Domain/          # Core domain entities
│   ├── FederationPlatform.Application/     # Business logic & DTOs
│   ├── FederationPlatform.Infrastructure/  # Data access & EF Core
│   └── FederationPlatform.Web/             # MVC Web Application
├── tests/
│   ├── FederationPlatform.UnitTests/
│   └── FederationPlatform.IntegrationTests/
└── FederationPlatform.sln
```

### Getting Started

#### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or Rider

#### Installation
1. Clone the repository
2. Open `FederationPlatform.sln` in Visual Studio
3. Restore NuGet packages
4. Update connection string in `appsettings.json`
5. Run migrations: `dotnet ef database update`
6. Run the application: `dotnet run --project src/FederationPlatform.Web`

### Features
- User management (Admin, Representative, User roles)
- Activity registration and approval workflow
- University and organization management
- Dashboard with analytics and charts
- News and workshop management
- File upload and management
- Persian calendar support
- RTL (Right-to-Left) UI

### Documentation
- [Requirements](../REQUIREMENTS.md)
- [Design](../DESIGN.md)
- [Tasks](../TASKS.md)

### License
Proprietary - Federation of Health Economy

### Contact
For more information, contact the Federation of Health Economy Student Committee.
