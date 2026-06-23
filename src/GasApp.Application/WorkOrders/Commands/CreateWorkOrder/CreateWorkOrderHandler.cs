using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderHandler(
    IWorkOrderRepository workOrderRepo,
    ILocationRepository locationRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWorkOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        _ = await locationRepo.GetByIdAsync(request.LocationId, cancellationToken)
            ?? throw new NotFoundException("Sede", request.LocationId);

        var existing = await workOrderRepo.GetByNumberAsync(request.OrderNumber, cancellationToken);
        if (existing != null)
            throw new DomainException($"Ya existe una orden con número {request.OrderNumber}.");

        var workOrder = WorkOrder.Create(request.OrderNumber, request.LocationId,
            request.InspectionTypeId, request.ScheduledDate, request.Notes);

        await workOrderRepo.AddAsync(workOrder, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workOrder.Id;
    }
}
