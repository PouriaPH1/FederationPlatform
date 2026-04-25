using FluentValidation;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Validators;

public class CreateWorkshopDtoValidator : AbstractValidator<CreateWorkshopDto>
{
    public CreateWorkshopDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان کارگاه الزامی است.")
            .MaximumLength(200).WithMessage("عنوان نباید بیش از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("توضیحات کارگاه الزامی است.")
            .MinimumLength(20).WithMessage("توضیحات باید حداقل ۲۰ کاراکتر باشد.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("مکان برگزاری الزامی است.")
            .MaximumLength(200).WithMessage("مکان نباید بیش از ۲۰۰ کاراکتر باشد.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع الزامی است.")
            .GreaterThan(DateTime.Now).WithMessage("تاریخ شروع باید در آینده باشد.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاریخ پایان الزامی است.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد.");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("حداکثر ظرفیت باید بزرگتر از صفر باشد.");
    }
}
