using MediatR;

namespace GasApp.Application.WorkOrders.Commands.CreateWorkOrder;

public record CreateWorkOrderCommand(
    string OrderNumber,
    Guid LocationId,
    Guid InspectionTypeId,
    DateTime ScheduledDate,
    string? Notes
) : IRequest<Guid>;
