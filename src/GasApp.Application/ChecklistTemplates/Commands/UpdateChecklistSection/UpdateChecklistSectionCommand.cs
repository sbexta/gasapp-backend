using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistSection;

public record UpdateChecklistSectionCommand(Guid SectionId, string Name, int Order) : IRequest;
