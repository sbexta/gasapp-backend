using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.AddChecklistItem;

public record AddChecklistItemCommand(
    Guid SectionId,
    string Question,
    string ItemType,
    int Order,
    bool IsRequired = true,
    string? HelpText = null) : IRequest<Guid>;
