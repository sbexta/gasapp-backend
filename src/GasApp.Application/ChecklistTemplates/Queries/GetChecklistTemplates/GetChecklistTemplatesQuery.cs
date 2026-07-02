using MediatR;

namespace GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplates;

public record GetChecklistTemplatesQuery : IRequest<IReadOnlyList<ChecklistTemplateListDto>>;

public record ChecklistTemplateListDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? InspectionTypeId,
    string? InspectionTypeName,
    bool IsActive,
    int SectionCount);
