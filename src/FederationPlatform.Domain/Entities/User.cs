using FederationPlatform.Domain.Enums;

namespace FederationPlatform.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public UserProfile? UserProfile { get; set; }
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<News> CreatedNews { get; set; } = new List<News>();
    public ICollection<Workshop> CreatedWorkshops { get; set; } = new List<Workshop>();
}
