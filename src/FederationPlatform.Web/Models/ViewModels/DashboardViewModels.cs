using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Web.Models.ViewModels;

public class DashboardViewModel
{
    public string? UserRole { get; set; }
    public UserDashboardViewModel? UserDashboard { get; set; }
    public RepresentativeDashboardViewModel? RepresentativeDashboard { get; set; }
    public AdminDashboardViewModel? AdminDashboard { get; set; }
}

public class UserDashboardViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<NewsItemViewModel>? RecentNews { get; set; }
    public List<WorkshopItemViewModel>? UpcomingWorkshops { get; set; }
    public List<UniversityBriefViewModel>? Universities { get; set; }
    public List<ActivityDetailViewModel>? MyActivities { get; set; }
    public List<ActivityDetailViewModel>? JoinedActivities { get; set; }
    public int TotalActivities { get; set; }
    public int PendingActivities { get; set; }
}

public class RepresentativeDashboardViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UniversityName { get; set; }
    public List<ActivityDetailViewModel>? MyActivities { get; set; }
    public int PendingActivityCount { get; set; }
    public int ApprovedActivityCount { get; set; }
    public int RejectedActivityCount { get; set; }
    public int TotalActivityCount { get; set; }
    public List<NewsItemViewModel>? RecentNews { get; set; }
    public List<WorkshopItemViewModel>? UpcomingWorkshops { get; set; }
}

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalActivities { get; set; }
    public int PendingActivities { get; set; }
    public int PendingActivitiesCount { get; set; }
    public int ApprovedActivities { get; set; }
    public int RejectedActivities { get; set; }
    public int TotalUniversities { get; set; }
    public int TotalOrganizations { get; set; }
    public List<PendingActivityViewModel>? PendingActivityList { get; set; }
    public List<UserAdminViewModel>? UsersList { get; set; }
    public List<UserAdminViewModel>? RecentUsers { get; set; }
    public List<ActivityStatViewModel>? ActivityStats { get; set; }
    public List<ActivityStatViewModel>? Statistics { get; set; }
    public Dictionary<string, int>? ActivityTypeStats { get; set; }
    public Dictionary<string, int>? UniversityActivityStats { get; set; }
    public ManageNewsViewModel? ManageNews { get; set; }
}

public class PendingActivityViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? UniversityName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public ActivityType ActivityType { get; set; }
}

public class UserAdminViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public string? UniversityName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime RegisterDate { get; set; }
}

public class ActivityStatViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ApproveActivityViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string? UniversityName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ActivityFileViewModel>? Files { get; set; }
}

public class ManageNewsViewModel
{
    public List<NewsItemViewModel>? NewsList { get; set; }
    public int TotalCount { get; set; }
}

public class CreateNewsViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}

public class ManageWorkshopsViewModel
{
    public List<WorkshopItemViewModel>? WorkshopList { get; set; }
    public int TotalCount { get; set; }
}

public class CreateWorkshopViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? RegistrationLink { get; set; }
}
