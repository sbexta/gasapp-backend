using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.StartWorkOrder;

public class StartWorkOrderHandler(
    IWorkOrderRepository workOrderRepo,
    IInspectionRepository inspectionRepo,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<StartWorkOrderCommand>
{
    public async Task Handle(StartWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepo.GetByIdAsync(request.WorkOrderId, cancellationToken)
            ?? throw new NotFoundException("Orden de trabajo", request.WorkOrderId);

        if (currentUser.Role == UserRole.Technician &&
            workOrder.AssignedTechnicianId != currentUser.UserId)
            throw new DomainException("Solo el técnico asignado puede iniciar esta orden.");

        workOrder.Start();

        // Crear la inspección si no existe ya
        var existing = await inspectionRepo.GetByWorkOrderIdAsync(workOrder.Id, cancellationToken);
        if (existing is null)
        {
            var techId = workOrder.AssignedTechnicianId
                ?? throw new DomainException("La orden debe tener un técnico asignado.");

            var inspection = Inspection.Create(workOrder.Id, techId);
            // Pending → PreCheck → InProgress para habilitar el flujo del técnico
            inspection.TransitionStatus(InspectionStatus.PreCheck, UserRole.Technician);
            inspection.TransitionStatus(InspectionStatus.InProgress, UserRole.Technician);
            await inspectionRepo.AddAsync(inspection, cancellationToken);
        }

        workOrderRepo.Update(workOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
