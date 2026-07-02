using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.AddChecklistSection;

public record AddChecklistSectionCommand(Guid TemplateId, string Name, int Order) : IRequest<Guid>;
