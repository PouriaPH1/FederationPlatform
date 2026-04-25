namespace FederationPlatform.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalRepresentatives { get; set; }
    public int TotalActivities { get; set; }
    public int PendingActivities { get; set; }
    public int ApprovedActivities { get; set; }
    public int TotalNews { get; set; }
    public int TotalWorkshops { get; set; }
    public int TotalUniversities { get; set; }
    public int TotalOrganizations { get; set; }

    public IList<UniversityActivityStatDto> TopUniversities { get; set; } = new List<UniversityActivityStatDto>();
    public IList<OrganizationActivityStatDto> TopOrganizations { get; set; } = new List<OrganizationActivityStatDto>();
    public IList<ActivityTypeStatDto> ActivityTypeBreakdown { get; set; } = new List<ActivityTypeStatDto>();
    public IList<MonthlyActivityStatDto> MonthlyActivityTrend { get; set; } = new List<MonthlyActivityStatDto>();

    public IList<ActivityListDto> RecentActivities { get; set; } = new List<ActivityListDto>();
    public IList<NewsDto> RecentNews { get; set; } = new List<NewsDto>();
    public IList<WorkshopDto> UpcomingWorkshops { get; set; } = new List<WorkshopDto>();
}

public class UniversityActivityStatDto
{
    public int UniversityId { get; set; }
    public string UniversityName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
}

public class OrganizationActivityStatDto
{
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
}

public class ActivityTypeStatDto
{
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyActivityStatDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RepresentativeDashboardDto
{
    public UserProfileDto? Profile { get; set; }
    public int MyActivityCount { get; set; }
    public int MyPendingCount { get; set; }
    public int MyApprovedCount { get; set; }
    public IList<ActivityListDto> MyRecentActivities { get; set; } = new List<ActivityListDto>();
    public IList<NewsDto> LatestNews { get; set; } = new List<NewsDto>();
    public IList<WorkshopDto> UpcomingWorkshops { get; set; } = new List<WorkshopDto>();
}

public class UserDashboardDto
{
    public IList<NewsDto> LatestNews { get; set; } = new List<NewsDto>();
    public IList<WorkshopDto> UpcomingWorkshops { get; set; } = new List<WorkshopDto>();
    public IList<UniversityDto> Universities { get; set; } = new List<UniversityDto>();
    public IList<ActivityListDto> RecentActivities { get; set; } = new List<ActivityListDto>();
}
