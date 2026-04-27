using FederationPlatform.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    [Display(Name = "دسته‌بندی")]
    public string? Category { get; set; }

    [Display(Name = "مکان")]
    public string? Location { get; set; }

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

    [Display(Name = "تعداد شرکت‌کنندگان پیش‌بینی‌شده")]
    [Range(1, 10000, ErrorMessage = "تعداد شرکت‌کنندگان معتبر نیست")]
    public int? ExpectedParticipants { get; set; }

    [Display(Name = "بودجه")]
    [Range(0, long.MaxValue, ErrorMessage = "بودجه معتبر نیست")]
    public decimal? Budget { get; set; }

    [Display(Name = "فایل‌ها")]
    public IFormFileCollection? Files { get; set; }

    public List<SelectListItem>? Universities { get; set; }
    public List<SelectListItem>? Organizations { get; set; }
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
    public string? Location { get; set; }
    public int ParticipantsCount { get; set; }
    public bool IsApproved { get; set; }
    public string? UniversityName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CreatedByName { get; set; }
    public string? RepresentativeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ActivityFileViewModel>? Files { get; set; }
}

public class ActivityListViewModel
{
    public List<ActivityDetailViewModel>? Activities { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int? UniversityId { get; set; }
    public int? OrganizationId { get; set; }
    public ActivityStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
    public List<SelectListItem>? Universities { get; set; }
    public List<SelectListItem>? Organizations { get; set; }
}

public class ActivityFileViewModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class MyActivitiesViewModel
{
    public List<ActivityCardViewModel>? PendingActivities { get; set; }
    public List<ActivityCardViewModel>? ApprovedActivities { get; set; }
    public List<ActivityCardViewModel>? RejectedActivities { get; set; }
    public List<ActivityCardViewModel>? Activities { get; set; }
}

public class ActivityCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RepresentativeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? UniversityName { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ParticipantsCount { get; set; }
}
