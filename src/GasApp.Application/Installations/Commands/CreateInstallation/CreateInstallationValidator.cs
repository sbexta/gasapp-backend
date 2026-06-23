using FluentValidation;

namespace GasApp.Application.Installations.Commands.CreateInstallation;

public class CreateInstallationValidator : AbstractValidator<CreateInstallationCommand>
{
    public CreateInstallationValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.InstallationYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .When(x => x.InstallationYear.HasValue);
    }
}
