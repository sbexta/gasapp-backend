using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplates;

public class GetChecklistTemplatesHandler(IChecklistTemplateRepository repo, IInspectionTypeRepository inspectionTypeRepo)
    : IRequestHandler<GetChecklistTemplatesQuery, IReadOnlyList<ChecklistTemplateListDto>>
{
    public async Task<IReadOnlyList<ChecklistTemplateListDto>> Handle(
        GetChecklistTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await repo.GetAllAsync(cancellationToken);
        var inspectionTypes = await inspectionTypeRepo.GetAllActiveAsync(cancellationToken);
        var typeMap = inspectionTypes.ToDictionary(t => t.Id, t => t.Name);

        return templates.Select(t => new ChecklistTemplateListDto(
            t.Id, t.Name, t.Description, t.InspectionTypeId,
            t.InspectionTypeId.HasValue ? typeMap.GetValueOrDefault(t.InspectionTypeId.Value) : null,
            t.IsActive,
            t.Sections.Count)).ToList();
    }
}
