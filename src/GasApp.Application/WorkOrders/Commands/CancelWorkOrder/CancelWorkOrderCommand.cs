using MediatR;

namespace GasApp.Application.WorkOrders.Commands.CancelWorkOrder;

public record CancelWorkOrderCommand(Guid Id, string Reason) : IRequest;
