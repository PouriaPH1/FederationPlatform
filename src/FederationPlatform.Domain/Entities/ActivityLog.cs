namespace FederationPlatform.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Login, Logout, Create, Update, Delete, Approve, Reject
    public string EntityType { get; set; } = string.Empty; // Activity, User, News, etc.
    public int? EntityId { get; set; }
    public string Details { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    // Navigation properties
    public User? User { get; set; }
}
