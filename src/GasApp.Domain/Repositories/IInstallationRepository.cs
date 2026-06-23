using GasApp.Domain.Entities.Clients;

namespace GasApp.Domain.Repositories;

public interface IInstallationRepository
{
    Task<Installation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Installation>> GetByLocationIdAsync(Guid locationId, CancellationToken ct = default);
    Task AddAsync(Installation installation, CancellationToken ct = default);
    void Update(Installation installation);
}
