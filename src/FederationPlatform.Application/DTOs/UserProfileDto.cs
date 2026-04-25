namespace FederationPlatform.Application.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int? UniversityId { get; set; }
    public string? UniversityName { get; set; }
    public string? Faculty { get; set; }
    public string? Major { get; set; }
    public int? EnrollmentYear { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ResumeUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
}

public class UpdateUserProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? UniversityId { get; set; }
    public string? Faculty { get; set; }
    public string? Major { get; set; }
    public int? EnrollmentYear { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
}
