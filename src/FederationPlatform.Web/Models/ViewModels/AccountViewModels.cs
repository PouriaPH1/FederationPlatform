using System.ComponentModel.DataAnnotations;

namespace FederationPlatform.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [Display(Name = "نام کاربری")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا یاد بسپار")]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین 3 تا 100 کاراکتر باشد")]
    [Display(Name = "نام کاربری")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
    [Display(Name = "ایمیل")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "تکرار رمز عبور")]
    [Compare("Password", ErrorMessage = "رمز عبورها مطابقت ندارند")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

public class ProfileViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "نام الزامی است")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "دانشگاه")]
    public int? UniversityId { get; set; }

    [Display(Name = "دانشکده")]
    public string? Faculty { get; set; }

    [Display(Name = "رشته تحصیلی")]
    public string? Major { get; set; }

    [Display(Name = "سال ورود")]
    public int? EnrollmentYear { get; set; }

    [Display(Name = "سمت")]
    public string? Position { get; set; }

    [Phone(ErrorMessage = "شماره تماس معتبر نیست")]
    [Display(Name = "شماره تماس")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "عکس پروفایل")]
    public IFormFile? ProfileImage { get; set; }

    [Display(Name = "رزومه")]
    public IFormFile? Resume { get; set; }

    public string? ProfileImageUrl { get; set; }
    public string? ResumeUrl { get; set; }
    public Dictionary<int, string>? Universities { get; set; }
}
