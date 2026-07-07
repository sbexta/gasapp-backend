using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistTemplate;

public record UpdateChecklistTemplateCommand(Guid TemplateId, string Name, string? Description) : IRequest;
