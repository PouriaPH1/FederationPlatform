using FluentValidation;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Validators;

public class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
{
    public CreateActivityDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان فعالیت الزامی است.")
            .MinimumLength(5).WithMessage("عنوان باید حداقل ۵ کاراکتر باشد.")
            .MaximumLength(200).WithMessage("عنوان نباید بیش از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("توضیحات فعالیت الزامی است.")
            .MinimumLength(20).WithMessage("توضیحات باید حداقل ۲۰ کاراکتر باشد.");

        RuleFor(x => x.UniversityId)
            .GreaterThan(0).WithMessage("انتخاب دانشگاه الزامی است.");

        RuleFor(x => x.OrganizationId)
            .GreaterThan(0).WithMessage("انتخاب تشکل الزامی است.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع الزامی است.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاریخ پایان الزامی است.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد.");
    }
}
