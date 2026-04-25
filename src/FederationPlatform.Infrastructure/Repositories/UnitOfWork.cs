using FederationPlatform.Application.Interfaces;
using FederationPlatform.Infrastructure.Data;

namespace FederationPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IUserProfileRepository> _userProfileRepository;
    private readonly Lazy<IUniversityRepository> _universityRepository;
    private readonly Lazy<IOrganizationRepository> _organizationRepository;
    private readonly Lazy<IActivityRepository> _activityRepository;
    private readonly Lazy<IActivityFileRepository> _activityFileRepository;
    private readonly Lazy<INewsRepository> _newsRepository;
    private readonly Lazy<IWorkshopRepository> _workshopRepository;
    private readonly Lazy<INotificationRepository> _notificationRepository;
    private readonly Lazy<IFeedbackRepository> _feedbackRepository;
    private readonly Lazy<IActivityLogRepository> _activityLogRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        _userProfileRepository = new Lazy<IUserProfileRepository>(() => new UserProfileRepository(context));
        _universityRepository = new Lazy<IUniversityRepository>(() => new UniversityRepository(context));
        _organizationRepository = new Lazy<IOrganizationRepository>(() => new OrganizationRepository(context));
        _activityRepository = new Lazy<IActivityRepository>(() => new ActivityRepository(context));
        _activityFileRepository = new Lazy<IActivityFileRepository>(() => new ActivityFileRepository(context));
        _newsRepository = new Lazy<INewsRepository>(() => new NewsRepository(context));
        _workshopRepository = new Lazy<IWorkshopRepository>(() => new WorkshopRepository(context));
        _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(context));
        _feedbackRepository = new Lazy<IFeedbackRepository>(() => new FeedbackRepository(context));
        _activityLogRepository = new Lazy<IActivityLogRepository>(() => new ActivityLogRepository(context));
    }

    public IUserRepository Users => _userRepository.Value;
    public IUserProfileRepository UserProfiles => _userProfileRepository.Value;
    public IUniversityRepository Universities => _universityRepository.Value;
    public IOrganizationRepository Organizations => _organizationRepository.Value;
    public IActivityRepository Activities => _activityRepository.Value;
    public IActivityFileRepository ActivityFiles => _activityFileRepository.Value;
    public INewsRepository News => _newsRepository.Value;
    public IWorkshopRepository Workshops => _workshopRepository.Value;
    public INotificationRepository Notifications => _notificationRepository.Value;
    public IFeedbackRepository Feedbacks => _feedbackRepository.Value;
    public IActivityLogRepository ActivityLogs => _activityLogRepository.Value;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _context.Database.RollbackTransactionAsync();
        }
        finally
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
