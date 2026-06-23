using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class InspectionType : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool RequiresCertificate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private InspectionType() { }

    public static InspectionType Create(string name, string? description = null, bool requiresCertificate = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del tipo de inspección no puede estar vacío.");

        return new InspectionType
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            RequiresCertificate = requiresCertificate
        };
    }

    public void Update(string name, string? description, bool requiresCertificate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del tipo de inspección no puede estar vacío.");

        Name = name.Trim();
        Description = description?.Trim();
        RequiresCertificate = requiresCertificate;
        Touch();
    }

    public void Activate() { IsActive = true; Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
}
