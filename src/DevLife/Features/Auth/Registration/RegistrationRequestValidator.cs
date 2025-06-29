using DevLife.Infrastructure.Models.Enums;
using FluentValidation;

namespace DevLife.Features.Auth.Registration;

public class RegistrationRequestValidator : AbstractValidator<Auth.Registration.Registration.RegistrationRequest>
{
    public RegistrationRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required")
            .MinimumLength(4).WithMessage("UserName must be at least 4 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required")
            .Must(x => x < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("You can't birth tomorrow nigga");

        RuleFor(x => x.TechStack)
            .NotEmpty().WithMessage("TechStack is required")
            .Must(ts => !string.IsNullOrWhiteSpace(ts.ToString()))
            .WithMessage("TechStack is invalid");

        RuleFor(x => x.ExperienceLevel)
            .NotEmpty().WithMessage("ExperienceLevel is required")
            .Must(x => new[] { ExperienceLevel.Junior, ExperienceLevel.Middle, ExperienceLevel.Senior}
                .Contains(x))
            .WithMessage("ExperienceLevel must be one of: Junior, Middle or Senior");
    }
}