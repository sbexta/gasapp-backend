using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.StartWorkOrder;

public class StartWorkOrderHandler(
    IWorkOrderRepository workOrderRepo,
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

        workOrderRepo.Update(workOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
