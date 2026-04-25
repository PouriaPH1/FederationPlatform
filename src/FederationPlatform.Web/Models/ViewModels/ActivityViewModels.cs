using FederationPlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FederationPlatform.Web.Models.ViewModels;

public class CreateActivityViewModel
{
    [Required(ErrorMessage = "عنوان فعالیت الزامی است")]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "توضیحات الزامی است")]
    [Display(Name = "توضیحات")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع فعالیت الزامی است")]
    [Display(Name = "نوع فعالیت")]
    public ActivityType ActivityType { get; set; }

    [Required(ErrorMessage = "دانشگاه الزامی است")]
    [Display(Name = "دانشگاه")]
    public int UniversityId { get; set; }

    [Required(ErrorMessage = "تشکل الزامی است")]
    [Display(Name = "تشکل")]
    public int OrganizationId { get; set; }

    [Required(ErrorMessage = "تاریخ شروع الزامی است")]
    [Display(Name = "تاریخ شروع")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "تاریخ پایان الزامی است")]
    [Display(Name = "تاریخ پایان")]
    public DateTime EndDate { get; set; }

    [Display(Name = "فایل‌ها")]
    public IFormFileCollection? Files { get; set; }

    public Dictionary<int, string>? Universities { get; set; }
    public Dictionary<int, string>? Organizations { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ActivityDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public ActivityStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? UniversityName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ActivityFileViewModel>? Files { get; set; }
}

public class ActivityListViewModel
{
    public List<ActivityDetailViewModel>? Activities { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UniversityId { get; set; }
    public int? OrganizationId { get; set; }
    public ActivityStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
    public Dictionary<int, string>? Universities { get; set; }
    public Dictionary<int, string>? Organizations { get; set; }
}

public class ActivityFileViewModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class MyActivitiesViewModel
{
    public List<ActivityDetailViewModel>? PendingActivities { get; set; }
    public List<ActivityDetailViewModel>? ApprovedActivities { get; set; }
    public List<ActivityDetailViewModel>? RejectedActivities { get; set; }
}
