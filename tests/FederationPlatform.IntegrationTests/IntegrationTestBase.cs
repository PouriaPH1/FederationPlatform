using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using FederationPlatform.Infrastructure.Data;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.Repositories;
using FederationPlatform.Infrastructure.Security;
using AutoMapper;
using FederationPlatform.Application.Mappings;

namespace FederationPlatform.IntegrationTests
{
    /// <summary>
    /// Base class for integration tests providing common setup and teardown
    /// </summary>
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected ApplicationDbContext _dbContext;
        protected IMapper _mapper;
        
        // Services
        protected IAuthService _authService;
        protected IUserService _userService;
        protected IActivityService _activityService;
        protected IUniversityService _universityService;
        protected INotificationService _notificationService;
        protected IUserProfileService _userProfileService;
        protected IAdminService _adminService;
        protected IReportService _reportService;
        protected IEmailService _emailService;
        
        // Repositories
        protected IUserRepository _userRepository;
        protected IActivityRepository _activityRepository;
        protected IUniversityRepository _universityRepository;
        protected INotificationRepository _notificationRepository;
        
        // Utilities
        protected SecurityHelper _securityHelper;
        protected IMenuService _menuService;
        protected ITokenService _tokenService;

        public async Task InitializeAsync()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            await _dbContext.Database.EnsureCreatedAsync();

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            // Setup Repositories
            _userRepository = new UserRepository(_dbContext);
            _activityRepository = new ActivityRepository(_dbContext);
            _universityRepository = new UniversityRepository(_dbContext);
            _notificationRepository = new NotificationRepository(_dbContext);

            // Setup Services
            _emailService = new Mock<IEmailService>().Object;
            _authService = new AuthService(_userRepository, _emailService);
            _userService = new UserService(_userRepository, new UserProfileRepository(_dbContext));
            _activityService = new ActivityService(_activityRepository, _userRepository, _notificationService);
            _universityService = new UniversityService(_universityRepository, _activityRepository);
            _notificationService = new NotificationService(_notificationRepository, _emailService);
            _userProfileService = new UserProfileService(new UserProfileRepository(_dbContext), _userRepository);
            _adminService = new AdminService(_userRepository, _activityRepository);
            _reportService = new ReportService(_activityRepository, _userRepository, _universityRepository);

            // Setup Utilities
            _securityHelper = new SecurityHelper();
            _menuService = new MenuService();
            _tokenService = new TokenService();

            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _dbContext.Dispose();
        }

        protected virtual async Task SeedTestDataAsync()
        {
            // Seed universities
            var universities = new[]
            {
                new University { Name = "University of Tehran", Province = "Tehran", City = "Tehran", IsActive = true },
                new University { Name = "Sharif University", Province = "Tehran", City = "Tehran", IsActive = true },
                new University { Name = "Isfahan University", Province = "Isfahan", City = "Isfahan", IsActive = true }
            };

            await _dbContext.Universities.AddRangeAsync(universities);
            await _dbContext.SaveChangesAsync();
        }

        protected async Task<User> CreateTestUserAsync(string email, string password)
        {
            var dto = new CreateUserDto
            {
                Username = email.Split('@')[0],
                Email = email,
                Password = password,
                FirstName = "Test",
                LastName = "User"
            };

            var result = await _authService.RegisterAsync(dto);
            return await _userService.GetUserByIdAsync(result.Data);
        }

        protected async Task<User> CreateTestRepresentativeAsync(string email)
        {
            var user = await CreateTestUserAsync(email, "Password123!");
            await _userService.PromoteToRepresentativeAsync(user.Id);
            return await _userService.GetUserByIdAsync(user.Id);
        }

        protected async Task<User> CreateTestAdminAsync(string email)
        {
            var user = await CreateTestUserAsync(email, "Password123!");
            user.Role = UserRole.Admin;
            await _userRepository.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        protected async Task<University> CreateTestUniversityAsync()
        {
            var university = new University
            {
                Name = $"Test University {Guid.NewGuid()}",
                Province = "Tehran",
                City = "Tehran",
                IsActive = true
            };

            await _universityRepository.AddAsync(university);
            await _dbContext.SaveChangesAsync();
            return university;
        }

        protected async Task<Activity> CreateTestActivityAsync(int userId)
        {
            var university = await CreateTestUniversityAsync();

            var activity = new Activity
            {
                Title = $"Test Activity {Guid.NewGuid()}",
                Description = "Test activity description",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = userId,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Status = ActivityStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _activityRepository.AddAsync(activity);
            await _dbContext.SaveChangesAsync();
            return activity;
        }

        protected async Task AssignRepresentativeToUniversityAsync(int userId, int universityId)
        {
            var userProfile = new UserProfile
            {
                UserId = userId,
                UniversityId = universityId
            };

            await _dbContext.UserProfiles.AddAsync(userProfile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
