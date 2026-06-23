using FluentValidation;

namespace GasApp.Application.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderValidator()
    {
        RuleFor(x => x.OrderNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.InspectionTypeId).NotEmpty();
        RuleFor(x => x.ScheduledDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("La fecha programada no puede ser en el pasado.");
    }
}
