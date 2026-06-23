using GasApp.Domain.Entities.Clients;

namespace GasApp.Domain.Repositories;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Contract?> GetByNumberAsync(string contractNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Contract>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task AddAsync(Contract contract, CancellationToken ct = default);
    void Update(Contract contract);
}
