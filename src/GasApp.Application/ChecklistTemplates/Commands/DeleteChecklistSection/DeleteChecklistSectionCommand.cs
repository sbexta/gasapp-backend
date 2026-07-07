using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistSection;

public record DeleteChecklistSectionCommand(Guid SectionId) : IRequest;
