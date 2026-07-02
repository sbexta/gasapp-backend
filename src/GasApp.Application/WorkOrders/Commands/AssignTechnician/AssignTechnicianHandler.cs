using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.AssignTechnician;

public class AssignTechnicianHandler(
    IWorkOrderRepository workOrderRepo,
    IUserRepository userRepo,
    INotificationRepository notifRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignTechnicianCommand>
{
    public async Task Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepo.GetByIdAsync(request.WorkOrderId, cancellationToken)
            ?? throw new NotFoundException("Orden de trabajo", request.WorkOrderId);

        var technician = await userRepo.GetByIdAsync(request.TechnicianId, cancellationToken)
            ?? throw new NotFoundException("Usuario", request.TechnicianId);

        if (technician.Role != UserRole.Technician)
            throw new DomainException("El usuario asignado debe tener rol de Técnico.");

        if (!technician.IsActive)
            throw new DomainException("No se puede asignar un técnico inactivo.");

        workOrder.AssignTechnician(request.TechnicianId);

        var notif = Notification.Create(
            technician.Id,
            "Nueva orden de trabajo asignada",
            $"Se te asignó la orden {workOrder.OrderNumber} para el {workOrder.ScheduledDate:dd/MM/yyyy}.",
            "WorkOrderAssigned",
            workOrder.Id);
        await notifRepo.AddAsync(notif, cancellationToken);

        workOrderRepo.Update(workOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
