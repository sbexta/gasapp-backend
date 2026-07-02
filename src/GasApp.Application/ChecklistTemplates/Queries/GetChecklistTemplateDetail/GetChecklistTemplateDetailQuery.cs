using MediatR;

namespace GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplateDetail;

public record GetChecklistTemplateDetailQuery(Guid TemplateId) : IRequest<ChecklistTemplateDetailDto>;

public record ChecklistTemplateDetailDto(
    Guid Id, string Name, string? Description,
    Guid? InspectionTypeId, bool IsActive, int Version,
    IReadOnlyList<ChecklistSectionDetailDto> Sections);

public record ChecklistSectionDetailDto(
    Guid Id, string Name, int Order,
    IReadOnlyList<ChecklistItemDetailDto> Items);

public record ChecklistItemDetailDto(
    Guid Id, string Question, string ItemType,
    bool IsRequired, int Order, string? HelpText);
