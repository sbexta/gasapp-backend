using MediatR;

namespace GasApp.Application.WorkOrders.Commands.AssignTechnician;

public record AssignTechnicianCommand(Guid WorkOrderId, Guid TechnicianId) : IRequest;
