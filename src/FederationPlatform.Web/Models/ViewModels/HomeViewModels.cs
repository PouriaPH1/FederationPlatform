namespace FederationPlatform.Web.Models.ViewModels;

public class HomeViewModel
{
    public int TotalActivities { get; set; }
    public int ActivitiesCount { get; set; }
    public int TotalUniversities { get; set; }
    public int UniversitiesCount { get; set; }
    public int TotalOrganizations { get; set; }
    public int UsersCount { get; set; }
    public int NewsCount { get; set; }
    public int PendingActivities { get; set; }
    public List<UniversityBriefViewModel>? TopUniversities { get; set; }
    public List<UniversityBriefViewModel>? Universities { get; set; }
    public List<NewsItemViewModel>? RecentNews { get; set; }
    public List<NewsItemViewModel>? News { get; set; }
    public List<ActivityBriefViewModel>? RecentActivities { get; set; }
    public List<WorkshopItemViewModel>? UpcomingWorkshops { get; set; }
    public Dictionary<string, int>? ActivityTypeStats { get; set; }
}

public class UniversityBriefViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int ActivityCount { get; set; }
    public int ActivitiesCount { get; set; }
    public int MembersCount { get; set; }
    public int NewsCount { get; set; }
}

public class UniversityDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Phone { get; set; }
    public int ActivitiesCount { get; set; }
    public int MembersCount { get; set; }
    public int NewsCount { get; set; }
    public int WorkshopsCount { get; set; }
    public List<ActivityBriefViewModel>? Activities { get; set; }
    public List<UserBriefViewModel>? Representatives { get; set; }
    public int ApprovedActivityCount { get; set; }
    public int TotalActivityCount { get; set; }
    public List<ActivityBriefViewModel>? RecentActivities { get; set; }
    public List<NewsItemViewModel>? News { get; set; }
}

public class UniversityListViewModel
{
    public List<UniversityCardViewModel>? Universities { get; set; }
    public int TotalCount { get; set; }
    public string? SearchTerm { get; set; }
    public string? ProvinceName { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 1;
}

public class NewsItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime PublishedAt { get; set; }
    public string? CreatedByName { get; set; }
}

public class WorkshopItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? RegistrationLink { get; set; }
    public string? CreatedByName { get; set; }
}

public class UserBriefViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int ActivityCount { get; set; }
}
