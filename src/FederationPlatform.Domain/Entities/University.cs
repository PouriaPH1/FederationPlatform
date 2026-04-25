namespace FederationPlatform.Domain.Entities;

public class University : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
