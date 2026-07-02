using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdHandler(
    IWorkOrderRepository workOrderRepo,
    ILocationRepository locationRepo)
    : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderDetailDto>
{
    public async Task<WorkOrderDetailDto> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Orden de trabajo", request.Id);

        var location = await locationRepo.GetByIdWithClientAsync(workOrder.LocationId, cancellationToken)
            ?? throw new NotFoundException("Sede", workOrder.LocationId);

        return new WorkOrderDetailDto(
            workOrder.Id,
            workOrder.OrderNumber,
            workOrder.Status.ToString(),
            workOrder.ScheduledDate,
            workOrder.Notes,
            new WorkOrderLocationDto(
                location.Name,
                location.Address,
                location.Municipality,
                location.Department
            ),
            new WorkOrderClientDto(
                location.Contract.Client.BusinessName,
                location.Contract.Client.ContactPhone
            )
        );
    }
}
