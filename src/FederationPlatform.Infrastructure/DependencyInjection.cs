using FederationPlatform.Application.Interfaces;
using FederationPlatform.Infrastructure.Data;
using FederationPlatform.Infrastructure.Identity;
using FederationPlatform.Infrastructure.Repositories;
using FederationPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FederationPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

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
        services.AddScoped<IFileService, FileService>();

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
