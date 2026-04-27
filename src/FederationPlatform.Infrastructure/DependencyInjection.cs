using FederationPlatform.Application.Interfaces;
using FederationPlatform.Application.Services;
using FederationPlatform.Infrastructure.Data;
using FederationPlatform.Infrastructure.Identity;
using FederationPlatform.Infrastructure.Repositories;
using FederationPlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FederationPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add HttpContextAccessor
        services.AddHttpContextAccessor();

        // Add DbContext
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var databaseProvider = configuration["DatabaseProvider"] ?? "SqlServer";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                var sqliteConnectionString = configuration.GetConnectionString("SqliteConnection");
                var sqliteFallbackConnection = "Data Source=FederationPlatform.db";

                var effectiveSqliteConnection = !string.IsNullOrWhiteSpace(sqliteConnectionString)
                    ? sqliteConnectionString
                    : defaultConnectionString;

                // If environment variables inject a SQL Server-style DefaultConnection,
                // keep local development resilient by falling back to a file-based SQLite DB.
                if (effectiveSqliteConnection.Contains("Server=", StringComparison.OrdinalIgnoreCase))
                {
                    effectiveSqliteConnection = sqliteFallbackConnection;
                }

                options.UseSqlite(effectiveSqliteConnection);
                return;
            }

            options.UseSqlServer(defaultConnectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
            });
        });

        // Add Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUniversityRepository, UniversityRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityFileRepository, ActivityFileRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Identity Services
        services.AddScoped<IIdentityService, IdentityService>();

        // Add File Service
        services.AddScoped<Infrastructure.Services.IFileService, FileService>();
        services.AddScoped<Application.Services.IFileService, ApplicationFileService>();

        // Add Email Service
        services.AddScoped<Application.Services.IEmailService, EmailService>();

        return services;
    }

    public static async Task<IApplicationBuilder> UseInfrastructure(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await DbInitializer.InitializeAsync(context);
        }

        return app;
    }
}
