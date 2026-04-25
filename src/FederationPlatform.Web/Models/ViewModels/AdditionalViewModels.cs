using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Web.Models.ViewModels;

public class ActivityBriefViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ActivityStatus Status { get; set; }
}

public class UniversityCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int ActivityCount { get; set; }
    public int RepresentativeCount { get; set; }
}

public class CreateActivityRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UniversityId { get; set; }
    public int OrganizationId { get; set; }
    public int? ExpectedParticipants { get; set; }
    public decimal? Budget { get; set; }
    public int? RepresentativeId { get; set; }
}
