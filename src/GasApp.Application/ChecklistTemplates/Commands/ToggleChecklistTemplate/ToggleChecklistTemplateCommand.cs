using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.ToggleChecklistTemplate;

public record ToggleChecklistTemplateCommand(Guid TemplateId) : IRequest;
