using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistItem;

public record DeleteChecklistItemCommand(Guid ItemId) : IRequest;
