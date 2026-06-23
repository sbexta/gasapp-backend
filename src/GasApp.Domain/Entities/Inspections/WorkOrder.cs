using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class WorkOrder : AuditableEntity
{
    public string OrderNumber { get; private set; } = null!;
    public Guid LocationId { get; private set; }
    public Guid InspectionTypeId { get; private set; }
    public InspectionType InspectionType { get; private set; } = null!;
    public Guid? AssignedTechnicianId { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public WorkOrderStatus Status { get; private set; } = WorkOrderStatus.Draft;
    public string? Notes { get; private set; }

    private WorkOrder() { }

    public static WorkOrder Create(string orderNumber, Guid locationId, Guid inspectionTypeId,
        DateTime scheduledDate, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("El número de orden no puede estar vacío.");

        if (scheduledDate < DateTime.UtcNow.Date)
            throw new DomainException("La fecha programada no puede ser en el pasado.");

        return new WorkOrder
        {
            OrderNumber = orderNumber.Trim(),
            LocationId = locationId,
            InspectionTypeId = inspectionTypeId,
            ScheduledDate = scheduledDate,
            Notes = notes?.Trim()
        };
    }

    public void AssignTechnician(Guid technicianId)
    {
        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Cancelled)
            throw new DomainException("No se puede asignar técnico a una orden completada o cancelada.");

        AssignedTechnicianId = technicianId;
        Status = WorkOrderStatus.Assigned;
        Touch();
    }

    public void Schedule()
    {
        if (Status != WorkOrderStatus.Draft)
            throw new DomainException("Solo se puede programar una orden en borrador.");

        Status = WorkOrderStatus.Scheduled;
        Touch();
    }

    public void Start()
    {
        if (Status != WorkOrderStatus.Assigned)
            throw new DomainException("La orden debe estar asignada para iniciarla.");

        Status = WorkOrderStatus.InProgress;
        Touch();
    }

    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new DomainException("La orden debe estar en progreso para completarla.");

        Status = WorkOrderStatus.Completed;
        Touch();
    }

    public void Cancel(string reason)
    {
        if (Status == WorkOrderStatus.Completed)
            throw new DomainException("No se puede cancelar una orden ya completada.");

        Status = WorkOrderStatus.Cancelled;
        Notes = reason.Trim();
        Touch();
    }
}
