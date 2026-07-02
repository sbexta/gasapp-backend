using MediatR;

namespace GasApp.Application.WorkOrders.Commands.UpdateWorkOrder;

public record UpdateWorkOrderCommand(Guid Id, DateTime ScheduledDate, string? Notes) : IRequest;
