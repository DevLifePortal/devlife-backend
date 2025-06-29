using FluentValidation;

namespace DevLife.Features.DatingProfile.Setup;

public class RegisterProfileCommandValidator : AbstractValidator<RegisterProfileCommand>
{
    public RegisterProfileCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Bio).MaximumLength(500);
        RuleFor(x => x.TechStack).NotEmpty();
    }
}