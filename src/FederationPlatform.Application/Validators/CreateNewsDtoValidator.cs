using FluentValidation;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Validators;

public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
{
    public CreateNewsDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان خبر الزامی است.")
            .MaximumLength(300).WithMessage("عنوان نباید بیش از ۳۰۰ کاراکتر باشد.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("متن خبر الزامی است.")
            .MinimumLength(50).WithMessage("متن خبر باید حداقل ۵۰ کاراکتر باشد.");
    }
}
