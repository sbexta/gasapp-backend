using GasApp.Domain.Entities.Clients;

namespace GasApp.Domain.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Location>> GetByContractIdAsync(Guid contractId, CancellationToken ct = default);
    Task AddAsync(Location location, CancellationToken ct = default);
    void Update(Location location);
}
