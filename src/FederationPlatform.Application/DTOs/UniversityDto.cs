namespace FederationPlatform.Application.DTOs;

public class UniversityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
}

public class UniversityDetailDto : UniversityDto
{
    public string? Description { get; set; }
    public int ActivityCount { get; set; }
    public int RepresentativeCount { get; set; }
    public IList<ActivityListDto> RecentActivities { get; set; } = new List<ActivityListDto>();
}
