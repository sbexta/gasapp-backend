using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(
    Guid ItemId, string Question, string ItemType,
    int Order, bool IsRequired, string? HelpText) : IRequest;
