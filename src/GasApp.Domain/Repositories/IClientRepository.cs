using GasApp.Domain.Entities.Clients;

namespace GasApp.Domain.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Client?> GetByNitAsync(string nit, CancellationToken ct = default);
    Task<(IReadOnlyList<Client> Items, int TotalCount)> GetPagedAsync(int page, int pageSize,
        string? search = null, bool? isActive = null, CancellationToken ct = default);
    Task AddAsync(Client client, CancellationToken ct = default);
    void Update(Client client);
}
