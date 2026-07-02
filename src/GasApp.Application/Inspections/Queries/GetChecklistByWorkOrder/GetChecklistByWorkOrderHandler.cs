using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetChecklistByWorkOrder;

public class GetChecklistByWorkOrderHandler(
    IWorkOrderRepository workOrderRepo,
    IInspectionRepository inspectionRepo,
    IChecklistTemplateRepository templateRepo,
    IChecklistResponseRepository responseRepo)
    : IRequestHandler<GetChecklistByWorkOrderQuery, WorkOrderChecklistDto?>
{
    public async Task<WorkOrderChecklistDto?> Handle(GetChecklistByWorkOrderQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepo.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null) return null;

        var inspection = await inspectionRepo.GetByWorkOrderIdAsync(request.WorkOrderId, cancellationToken);
        if (inspection is null) return null;

        var template = await templateRepo.GetByInspectionTypeIdAsync(workOrder.InspectionTypeId, cancellationToken);
        if (template is null) return null;

        var responses = await responseRepo.GetByInspectionIdAsync(inspection.Id, cancellationToken);
        var responseMap = responses.ToDictionary(r => r.ChecklistItemId);

        var sections = template.Sections
            .OrderBy(s => s.Order)
            .Select(s => new ChecklistSectionDto(
                s.Id,
                s.Name,
                s.Order,
                s.Items.OrderBy(i => i.Order).Select(i =>
                {
                    responseMap.TryGetValue(i.Id, out var resp);
                    return new ChecklistItemDto(
                        i.Id, i.Question, i.ItemType.ToString(), i.IsRequired, i.Order, i.HelpText,
                        resp is null ? null : new ChecklistItemResponseDto(
                            resp.Id, resp.TextValue, resp.BoolValue, resp.NumericValue, resp.Complies, resp.Notes)
                    );
                }).ToList()
            )).ToList();

        return new WorkOrderChecklistDto(inspection.Id, sections);
    }
}
