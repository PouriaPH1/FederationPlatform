using FluentValidation;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("نام کاربری الزامی است.")
            .MinimumLength(3).WithMessage("نام کاربری باید حداقل ۳ کاراکتر باشد.")
            .MaximumLength(50).WithMessage("نام کاربری نباید بیش از ۵۰ کاراکتر باشد.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("نام کاربری فقط می‌تواند شامل حروف انگلیسی، اعداد و _ باشد.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است.")
            .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است.")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد.")
            .Matches("[A-Z]").WithMessage("رمز عبور باید حداقل یک حرف بزرگ داشته باشد.")
            .Matches("[0-9]").WithMessage("رمز عبور باید حداقل یک عدد داشته باشد.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است.")
            .Matches("^09[0-9]{9}$").WithMessage("فرمت شماره موبایل صحیح نیست. (مثال: 09123456789)");
    }
}
