using GasApp.Domain.Entities.Users;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class UserSessionRepository(AppDbContext context) : IUserSessionRepository
{
    public async Task<UserSession?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        await context.UserSessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == tokenHash, ct);

    public async Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

    public async Task AddAsync(UserSession session, CancellationToken ct = default) =>
        await context.UserSessions.AddAsync(session, ct);

    public void Update(UserSession session) =>
        context.UserSessions.Update(session);
}
