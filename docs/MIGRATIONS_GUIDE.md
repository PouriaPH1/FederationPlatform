# Entity Framework Migrations Guide

## Overview

This project uses Entity Framework Core 8.0 with SQL Server. Migrations are automatically created and applied when the application starts.

## Initial Migration

The first migration will be created automatically when the application starts for the first time. The `DbInitializer` class handles this:

```csharp
public static async Task InitializeAsync(ApplicationDbContext context)
{
    // Automatically runs pending migrations
    await context.Database.MigrateAsync();
    
    // Seeds initial data
    // ...
}
```

## Creating New Migrations

When you add or modify entities, create a new migration:

```bash
# Add a new migration
cd FederationPlatform/src/FederationPlatform.Infrastructure
dotnet ef migrations add "MigrationName" --startup-project ../FederationPlatform.Web

# Remove the last migration (if not applied)
dotnet ef migrations remove --startup-project ../FederationPlatform.Web
```

## Applying Migrations Manually

To apply migrations manually (not recommended as they run automatically on startup):

```bash
dotnet ef database update --startup-project ../FederationPlatform.Web
```

## Viewing Migrations

```bash
# List all migrations
dotnet ef migrations list

# See migration details
dotnet ef migrations script --from InitialCreate --to LatestMigration
```

## Reversing Migrations

To revert to a specific migration:

```bash
dotnet ef database update "PreviousMigrationName" --startup-project ../FederationPlatform.Web
```

## Database Connection String

The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FederationPlatformDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### Environment Variables

You can override the connection string via environment variables:

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "YOUR_CONNECTION_STRING"

# Linux/Mac
export ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"
```

## Seed Data

Initial seed data is automatically created in `DbInitializer`:

- **25 Universities** - All major medical universities in Iran
- **8 Organizations** - All member syndicates
- **1 Admin User** - Default admin with credentials admin/admin123

To modify seed data, edit `DbInitializer.cs`:

```csharp
private static List<University> GetUniversities()
{
    return new List<University>
    {
        // Add/modify universities here
    };
}
```

## Troubleshooting

### Migration errors

If migrations fail, check:

1. Connection string is correct
2. SQL Server is running
3. No pending migrations exist: `dotnet ef migrations list`

### Database exists error

If you get "database already exists" error:

```bash
# Drop the database (WARNING: This deletes all data)
dotnet ef database drop --startup-project ../FederationPlatform.Web

# Then run the application again to recreate it
```

### Automatic migration on startup issues

If the automatic migration in startup fails:

1. Check `DbInitializer.cs` in logs
2. Verify seed data values are valid
3. Check for unique constraint violations

---

**Note:** Always backup your database before making migration changes in production!
