using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Checklists;

public class ChecklistSection : AuditableEntity
{
    public Guid TemplateId { get; private set; }
    public ChecklistTemplate Template { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Order { get; private set; }

    private readonly List<ChecklistItem> _items = [];
    public IReadOnlyList<ChecklistItem> Items => _items.AsReadOnly();

    private ChecklistSection() { }

    public static ChecklistSection Create(Guid templateId, string name, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la sección no puede estar vacío.");

        return new ChecklistSection
        {
            TemplateId = templateId,
            Name = name.Trim(),
            Order = order
        };
    }

    public void Update(string name, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la sección no puede estar vacío.");

        Name = name.Trim();
        Order = order;
        Touch();
    }
}
