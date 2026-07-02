using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.CreateChecklistTemplate;

public record CreateChecklistTemplateCommand(
    string Name,
    string? Description,
    Guid? InspectionTypeId) : IRequest<Guid>;
