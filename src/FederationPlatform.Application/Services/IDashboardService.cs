using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetAdminDashboardAsync();
    Task<RepresentativeDashboardDto> GetRepresentativeDashboardAsync(int userId);
    Task<UserDashboardDto> GetUserDashboardAsync();
}
