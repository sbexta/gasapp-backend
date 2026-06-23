using GasApp.Domain.Entities.Users;

namespace GasApp.Domain.Repositories;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(UserSession session, CancellationToken ct = default);
    void Update(UserSession session);
}
