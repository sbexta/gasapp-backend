using GasApp.Domain.Entities.Users;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task<(IReadOnlyList<Notification> Items, int Total)> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Notifications.Where(n => n.UserId == userId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
        => context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct = default)
        => await context.Notifications.AddAsync(notification, ct);

    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default)
        => await context.Notifications.AddRangeAsync(notifications, ct);

    public void Update(Notification notification)
        => context.Notifications.Update(notification);
}
