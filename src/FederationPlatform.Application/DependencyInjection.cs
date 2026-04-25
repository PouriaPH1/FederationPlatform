using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FederationPlatform.Application.Services;

namespace FederationPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IWorkshopService, WorkshopService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<IEmailService, Infrastructure.Services.EmailService>();

        return services;
    }
}
