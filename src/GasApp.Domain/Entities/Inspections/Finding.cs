using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class Finding : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public string Description { get; private set; } = null!;
    public FindingSeverity Severity { get; private set; }
    public bool RequiresCorrection { get; private set; }
    public string? CorrectiveAction { get; private set; }
    public bool IsResolved { get; private set; }
    public Guid? ChecklistItemId { get; private set; }

    private Finding() { }

    public static Finding Create(Guid inspectionId, string description, FindingSeverity severity,
        bool requiresCorrection, Guid? checklistItemId = null, string? correctiveAction = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del hallazgo no puede estar vacía.");

        return new Finding
        {
            InspectionId = inspectionId,
            Description = description.Trim(),
            Severity = severity,
            RequiresCorrection = requiresCorrection,
            ChecklistItemId = checklistItemId,
            CorrectiveAction = correctiveAction?.Trim()
        };
    }

    public void Resolve(string? notes = null)
    {
        IsResolved = true;
        if (notes != null) CorrectiveAction = notes.Trim();
        Touch();
    }
}
