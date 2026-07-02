using MediatR;

namespace GasApp.Application.WorkOrders.Commands.StartWorkOrder;

public record StartWorkOrderCommand(Guid WorkOrderId) : IRequest;
