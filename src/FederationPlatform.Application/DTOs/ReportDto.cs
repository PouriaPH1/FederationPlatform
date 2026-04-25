namespace FederationPlatform.Application.DTOs;

public class ReportFilterDto
{
    public int? UniversityId { get; set; }
    public int? OrganizationId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

public class ActivityReportDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public string RepresentativeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int FileCount { get; set; }
}

public class UniversityReportDto
{
    public string UniversityName { get; set; } = string.Empty;
    public int TotalActivities { get; set; }
    public int ApprovedActivities { get; set; }
    public int PendingActivities { get; set; }
    public int RejectedActivities { get; set; }
    public decimal ApprovalRate { get; set; }
    public List<string> TopActivityTypes { get; set; } = new();
}

public class RepresentativeReportDto
{
    public string RepresentativeName { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public int TotalActivities { get; set; }
    public int ApprovedActivities { get; set; }
    public int PendingActivities { get; set; }
    public decimal ApprovalRate { get; set; }
    public List<ActivityReportDto> Activities { get; set; } = new();
}
