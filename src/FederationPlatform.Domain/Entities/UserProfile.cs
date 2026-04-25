namespace FederationPlatform.Domain.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? UniversityId { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public int? EnrollmentYear { get; set; }
    public string Position { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ResumeUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public University? University { get; set; }
}
