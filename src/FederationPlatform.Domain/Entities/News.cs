namespace FederationPlatform.Domain.Entities;

public class News : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public bool IsPublished { get; set; } = true;
    
    // Navigation properties
    public User Creator { get; set; } = null!;
}
