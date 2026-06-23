using FluentValidation;

namespace GasApp.Application.Clients.Commands.CreateClient;

public class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Nit).NotEmpty().MaximumLength(20)
            .Matches(@"^\d{9}-\d$").WithMessage("El NIT debe tener formato 123456789-0.");
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
        RuleFor(x => x.ContactPhone).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));
    }
}
