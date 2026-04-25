namespace FederationPlatform.Domain.Entities;

public class Workshop : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? RegistrationLink { get; set; }
    public int CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public int MaxParticipants { get; set; }
    
    // Navigation properties
    public User Creator { get; set; } = null!;
}
