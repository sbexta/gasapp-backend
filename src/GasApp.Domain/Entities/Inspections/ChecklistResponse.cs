using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class ChecklistResponse : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public Guid ChecklistItemId { get; private set; }
    public string? TextValue { get; private set; }
    public bool? BoolValue { get; private set; }
    public decimal? NumericValue { get; private set; }
    public string? Notes { get; private set; }
    public bool Complies { get; private set; } = true;

    private ChecklistResponse() { }

    public static ChecklistResponse Create(Guid inspectionId, Guid checklistItemId,
        string? textValue, bool? boolValue, decimal? numericValue, bool complies, string? notes = null)
    {
        if (inspectionId == Guid.Empty)
            throw new DomainException("La inspección es requerida.");

        if (checklistItemId == Guid.Empty)
            throw new DomainException("El ítem de checklist es requerido.");

        return new ChecklistResponse
        {
            InspectionId = inspectionId,
            ChecklistItemId = checklistItemId,
            TextValue = textValue?.Trim(),
            BoolValue = boolValue,
            NumericValue = numericValue,
            Complies = complies,
            Notes = notes?.Trim()
        };
    }

    public void Update(string? textValue, bool? boolValue, decimal? numericValue, bool complies, string? notes)
    {
        TextValue = textValue?.Trim();
        BoolValue = boolValue;
        NumericValue = numericValue;
        Complies = complies;
        Notes = notes?.Trim();
        Touch();
    }
}
