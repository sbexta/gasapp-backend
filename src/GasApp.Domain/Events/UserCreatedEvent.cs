using GasApp.Domain.Entities;
using GasApp.Domain.Enums;

namespace GasApp.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email, UserRole Role) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
