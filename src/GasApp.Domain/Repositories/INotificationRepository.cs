using GasApp.Domain.Entities.Users;

namespace GasApp.Domain.Repositories;

public interface INotificationRepository
{
    Task<(IReadOnlyList<Notification> Items, int Total)> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default);
    void Update(Notification notification);
}
