using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Domain.Entities;

public class Activity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public int UniversityId { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ActivityStatus Status { get; set; } = ActivityStatus.Pending;
    
    // Navigation properties
    public University University { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<ActivityFile> ActivityFiles { get; set; } = new List<ActivityFile>();
}
