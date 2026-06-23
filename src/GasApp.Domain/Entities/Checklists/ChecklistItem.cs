using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Checklists;

public class ChecklistItem : AuditableEntity
{
    public Guid SectionId { get; private set; }
    public ChecklistSection Section { get; private set; } = null!;
    public string Question { get; private set; } = null!;
    public ChecklistItemType ItemType { get; private set; }
    public bool IsRequired { get; private set; } = true;
    public int Order { get; private set; }
    public string? HelpText { get; private set; }

    private ChecklistItem() { }

    public static ChecklistItem Create(Guid sectionId, string question, ChecklistItemType itemType,
        int order, bool isRequired = true, string? helpText = null)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new DomainException("La pregunta del ítem no puede estar vacía.");

        return new ChecklistItem
        {
            SectionId = sectionId,
            Question = question.Trim(),
            ItemType = itemType,
            Order = order,
            IsRequired = isRequired,
            HelpText = helpText?.Trim()
        };
    }

    public void Update(string question, ChecklistItemType itemType, int order, bool isRequired, string? helpText)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new DomainException("La pregunta del ítem no puede estar vacía.");

        Question = question.Trim();
        ItemType = itemType;
        Order = order;
        IsRequired = isRequired;
        HelpText = helpText?.Trim();
        Touch();
    }
}
