using GasApp.Application.Common.Models;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetWorkOrders;

public class GetWorkOrdersHandler(IWorkOrderRepository workOrderRepo)
    : IRequestHandler<GetWorkOrdersQuery, PagedResult<WorkOrderDto>>
{
    public async Task<PagedResult<WorkOrderDto>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await workOrderRepo.GetPagedAsync(
            request.Page, request.PageSize,
            request.Status, request.TechnicianId,
            request.From, request.To, cancellationToken);

        var dtos = items.Select(w => new WorkOrderDto(
            w.Id, w.OrderNumber, w.LocationId, w.AssignedTechnicianId,
            w.ScheduledDate, w.Status.ToString(), w.Notes)).ToList();

        return new PagedResult<WorkOrderDto>(dtos, total, request.Page, request.PageSize);
    }
}
