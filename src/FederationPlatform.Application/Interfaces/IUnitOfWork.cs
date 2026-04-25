namespace FederationPlatform.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserProfileRepository UserProfiles { get; }
    IUniversityRepository Universities { get; }
    IOrganizationRepository Organizations { get; }
    IActivityRepository Activities { get; }
    IActivityFileRepository ActivityFiles { get; }
    INewsRepository News { get; }
    IWorkshopRepository Workshops { get; }
    INotificationRepository Notifications { get; }
    IFeedbackRepository Feedbacks { get; }
    IActivityLogRepository ActivityLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
