using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Application.DTOs;

public class ActivityDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string ActivityTypeName { get; set; } = string.Empty;
    public int UniversityId { get; set; }
    public string UniversityName { get; set; } = string.Empty;
    public string? University { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string? Representative { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public int ParticipantCount { get; set; }
    public bool IsApproved { get; set; }
    public ActivityStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IList<ActivityFileDto> Files { get; set; } = new List<ActivityFileDto>();
}

public class ActivityListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public int? ParticipantCount { get; set; }
    public ActivityType ActivityType { get; set; }
    public string ActivityTypeName { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public int UniversityId { get; set; }
    public int OrganizationId { get; set; }
    public string? University { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? Representative { get; set; }
    public int? RepresentativeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsApproved { get; set; }
    public ActivityStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IList<ActivityFileDto>? Files { get; set; } = new List<ActivityFileDto>();
}

public class CreateActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public int UniversityId { get; set; }
    public int OrganizationId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public string? Category { get; set; }
    public int ExpectedParticipants { get; set; }
    public decimal Budget { get; set; }
}

public class UpdateActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public int UniversityId { get; set; }
    public int OrganizationId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ActivityFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class ActivityFilterDto
{
    public string? SearchTerm { get; set; }
    public int? UniversityId { get; set; }
    public int? OrganizationId { get; set; }
    public ActivityType? ActivityType { get; set; }
    public ActivityStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResultDto<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public class ActivityStatisticDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}
