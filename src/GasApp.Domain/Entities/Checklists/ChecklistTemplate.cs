using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Checklists;

public class ChecklistTemplate : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? InspectionTypeId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int Version { get; private set; } = 1;

    private readonly List<ChecklistSection> _sections = [];
    public IReadOnlyList<ChecklistSection> Sections => _sections.AsReadOnly();

    private ChecklistTemplate() { }

    public static ChecklistTemplate Create(string name, string? description = null, Guid? inspectionTypeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la plantilla no puede estar vacío.");

        return new ChecklistTemplate
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            InspectionTypeId = inspectionTypeId
        };
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la plantilla no puede estar vacío.");

        Name = name.Trim();
        Description = description?.Trim();
        Version++;
        Touch();
    }

    public void Activate() { IsActive = true; Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
}
