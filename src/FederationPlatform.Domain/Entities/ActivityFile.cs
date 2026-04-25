namespace FederationPlatform.Domain.Entities;

public class ActivityFile : BaseEntity
{
    public int ActivityId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Activity Activity { get; set; } = null!;
}
