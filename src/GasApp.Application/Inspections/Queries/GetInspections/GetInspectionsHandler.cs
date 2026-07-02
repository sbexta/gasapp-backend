using GasApp.Application.Common.Models;
using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspections;

public class GetInspectionsHandler(IInspectionRepository inspectionRepo)
    : IRequestHandler<GetInspectionsQuery, PagedResult<InspectionListDto>>
{
    public async Task<PagedResult<InspectionListDto>> Handle(GetInspectionsQuery request, CancellationToken cancellationToken)
    {
        InspectionStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<InspectionStatus>(request.Status, out var parsed))
            status = parsed;

        var (items, total) = await inspectionRepo.GetPagedAsync(request.Page, request.PageSize, status, cancellationToken);

        var dtos = items.Select(i => new InspectionListDto(
            i.Id,
            i.WorkOrderId,
            i.WorkOrder.OrderNumber,
            i.Status.ToString(),
            i.StartedAt,
            i.CompletedAt,
            i.WorkOrder.ScheduledDate
        )).ToList();

        return new PagedResult<InspectionListDto>(dtos, total, request.Page, request.PageSize);
    }
}
