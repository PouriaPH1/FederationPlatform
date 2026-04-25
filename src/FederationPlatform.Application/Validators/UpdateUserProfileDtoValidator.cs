using FluentValidation;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Validators;

public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("نام الزامی است.")
            .MaximumLength(50).WithMessage("نام نباید بیش از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("نام خانوادگی الزامی است.")
            .MaximumLength(50).WithMessage("نام خانوادگی نباید بیش از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.PhoneNumber)
            .Matches("^09[0-9]{9}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("فرمت شماره موبایل صحیح نیست.");

        RuleFor(x => x.EnrollmentYear)
            .InclusiveBetween(1370, 1420)
            .When(x => x.EnrollmentYear.HasValue)
            .WithMessage("سال ورود باید بین ۱۳۷۰ و ۱۴۲۰ باشد.");

        RuleFor(x => x.Faculty)
            .MaximumLength(100).WithMessage("نام دانشکده نباید بیش از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.Major)
            .MaximumLength(100).WithMessage("نام رشته نباید بیش از ۱۰۰ کاراکتر باشد.");
    }
}
