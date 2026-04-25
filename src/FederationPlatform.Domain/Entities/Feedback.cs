namespace FederationPlatform.Domain.Entities;

public class Feedback : BaseEntity
{
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;

    // Navigation properties
    public Activity Activity { get; set; } = null!;
    public User User { get; set; } = null!;
}
