namespace FederationPlatform.Application.DTOs;

public class WorkshopDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? RegistrationLink { get; set; }
    public bool IsActive { get; set; }
    public int MaxParticipants { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateWorkshopDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? RegistrationLink { get; set; }
    public int MaxParticipants { get; set; }
}

public class UpdateWorkshopDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? RegistrationLink { get; set; }
    public bool IsActive { get; set; }
    public int MaxParticipants { get; set; }
}
