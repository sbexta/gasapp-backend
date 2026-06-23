using GasApp.Domain.Entities;
using GasApp.Domain.Enums;

namespace GasApp.Domain.Events;

public record InspectionStatusChangedEvent(
    Guid InspectionId,
    InspectionStatus FromStatus,
    InspectionStatus ToStatus,
    Guid ChangedBy) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
