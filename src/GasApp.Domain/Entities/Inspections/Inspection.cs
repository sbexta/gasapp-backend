using GasApp.Domain.Enums;
using GasApp.Domain.Events;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Services;

namespace GasApp.Domain.Entities.Inspections;

public class Inspection : AuditableEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public Guid TechnicianId { get; private set; }
    public InspectionStatus Status { get; private set; } = InspectionStatus.Pending;
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? TechnicianNotes { get; private set; }
    public string? SupervisorNotes { get; private set; }

    private Inspection() { }

    public static Inspection Create(Guid workOrderId, Guid technicianId)
    {
        return new Inspection
        {
            WorkOrderId = workOrderId,
            TechnicianId = technicianId
        };
    }

    public void TransitionStatus(InspectionStatus target, UserRole performedByRole, string? notes = null)
    {
        InspectionStateMachine.ValidateTransition(Status, target, performedByRole);

        var previous = Status;
        Status = target;

        if (target == InspectionStatus.InProgress)
            StartedAt = DateTime.UtcNow;

        if (target == InspectionStatus.Completed)
            CompletedAt = DateTime.UtcNow;

        if (notes != null)
        {
            if (performedByRole == UserRole.Technician)
                TechnicianNotes = notes.Trim();
            else
                SupervisorNotes = notes.Trim();
        }

        AddDomainEvent(new InspectionStatusChangedEvent(Id, previous, target, TechnicianId));
        Touch();
    }
}
