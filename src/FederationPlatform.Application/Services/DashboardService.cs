using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetAdminDashboardAsync()
    {
        var allActivities = (await _unitOfWork.Activities.GetAllAsync()).ToList();
        var allUsers = (await _unitOfWork.Users.GetAllAsync()).ToList();
        var universities = (await _unitOfWork.Universities.GetAllAsync()).ToList();
        var organizations = (await _unitOfWork.Organizations.GetAllAsync()).ToList();

        var stats = new DashboardStatsDto
        {
            TotalUsers = allUsers.Count(u => u.Role == UserRole.User),
            TotalRepresentatives = allUsers.Count(u => u.Role == UserRole.Representative),
            TotalActivities = allActivities.Count,
            PendingActivities = allActivities.Count(a => a.Status == ActivityStatus.Pending),
            ApprovedActivities = allActivities.Count(a => a.Status == ActivityStatus.Approved),
            TotalNews = await _unitOfWork.News.CountAsync(),
            TotalWorkshops = await _unitOfWork.Workshops.CountAsync(),
            TotalUniversities = universities.Count,
            TotalOrganizations = organizations.Count
        };

        stats.TopUniversities = universities
            .Select(u => new UniversityActivityStatDto
            {
                UniversityId = u.Id,
                UniversityName = u.Name,
                ActivityCount = allActivities.Count(a => a.UniversityId == u.Id)
            })
            .OrderByDescending(u => u.ActivityCount)
            .Take(5)
            .ToList();

        stats.TopOrganizations = organizations
            .Select(o => new OrganizationActivityStatDto
            {
                OrganizationId = o.Id,
                OrganizationName = o.Name,
                ActivityCount = allActivities.Count(a => a.OrganizationId == o.Id)
            })
            .OrderByDescending(o => o.ActivityCount)
            .Take(5)
            .ToList();

        stats.ActivityTypeBreakdown = Enum.GetValues<ActivityType>()
            .Select(t => new ActivityTypeStatDto
            {
                TypeName = t.ToString(),
                Count = allActivities.Count(a => a.ActivityType == t)
            })
            .Where(t => t.Count > 0)
            .ToList();

        stats.MonthlyActivityTrend = allActivities
            .GroupBy(a => new { a.CreatedAt.Year, a.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .TakeLast(12)
            .Select(g => new MonthlyActivityStatDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                Count = g.Count()
            })
            .ToList();

        stats.RecentActivities = _mapper.Map<IList<ActivityListDto>>(
            allActivities.OrderByDescending(a => a.CreatedAt).Take(10).ToList());

        var latestNews = await _unitOfWork.News.GetLatestAsync(5);
        stats.RecentNews = _mapper.Map<IList<NewsDto>>(latestNews);

        var upcomingWorkshops = await _unitOfWork.Workshops.GetUpcomingAsync();
        stats.UpcomingWorkshops = _mapper.Map<IList<WorkshopDto>>(upcomingWorkshops.Take(5).ToList());

        return stats;
    }

    public async Task<RepresentativeDashboardDto> GetRepresentativeDashboardAsync(int userId)
    {
        var activities = (await _unitOfWork.Activities.GetByUserIdAsync(userId)).ToList();

        var dto = new RepresentativeDashboardDto
        {
            Profile = null,
            MyActivityCount = activities.Count,
            MyPendingCount = activities.Count(a => a.Status == ActivityStatus.Pending),
            MyApprovedCount = activities.Count(a => a.Status == ActivityStatus.Approved),
            MyRecentActivities = _mapper.Map<IList<ActivityListDto>>(
                activities.OrderByDescending(a => a.CreatedAt).Take(5).ToList())
        };

        var latestNews = await _unitOfWork.News.GetLatestAsync(5);
        dto.LatestNews = _mapper.Map<IList<NewsDto>>(latestNews);

        var upcomingWorkshops = await _unitOfWork.Workshops.GetUpcomingAsync();
        dto.UpcomingWorkshops = _mapper.Map<IList<WorkshopDto>>(upcomingWorkshops.Take(5).ToList());

        return dto;
    }

    public async Task<UserDashboardDto> GetUserDashboardAsync()
    {
        var latestNews = await _unitOfWork.News.GetLatestAsync(6);
        var upcomingWorkshops = await _unitOfWork.Workshops.GetUpcomingAsync();
        var universities = await _unitOfWork.Universities.GetActiveAsync();
        var approvedActivities = await _unitOfWork.Activities.GetApprovedAsync();

        return new UserDashboardDto
        {
            LatestNews = _mapper.Map<IList<NewsDto>>(latestNews),
            UpcomingWorkshops = _mapper.Map<IList<WorkshopDto>>(upcomingWorkshops.Take(5).ToList()),
            Universities = _mapper.Map<IList<UniversityDto>>(universities),
            RecentActivities = _mapper.Map<IList<ActivityListDto>>(
                approvedActivities.OrderByDescending(a => a.CreatedAt).Take(10).ToList())
        };
    }
}
