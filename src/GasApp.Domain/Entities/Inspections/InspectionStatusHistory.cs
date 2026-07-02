using GasApp.Domain.Enums;

namespace GasApp.Domain.Entities.Inspections;

public class InspectionStatusHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid InspectionId { get; private set; }
    public InspectionStatus? PreviousStatus { get; private set; }
    public InspectionStatus NewStatus { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid? ChangedById { get; private set; }
    public string? Notes { get; private set; }

    private InspectionStatusHistory() { }

    public static InspectionStatusHistory Create(
        Guid inspectionId, InspectionStatus? previousStatus, InspectionStatus newStatus,
        Guid? changedById, string? notes = null)
        => new()
        {
            InspectionId = inspectionId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow,
            ChangedById = changedById,
            Notes = notes
        };
}
