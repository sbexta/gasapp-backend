using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ContractRepository(AppDbContext context) : IContractRepository
{
    public async Task<Contract?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Contracts.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Contract?> GetByNumberAsync(string contractNumber, CancellationToken ct = default)
        => await context.Contracts.FirstOrDefaultAsync(c => c.ContractNumber == contractNumber, ct);

    public async Task<IReadOnlyList<Contract>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default)
        => await context.Contracts
            .Where(c => c.ClientId == clientId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Contract contract, CancellationToken ct = default)
        => await context.Contracts.AddAsync(contract, ct);

    public void Update(Contract contract)
        => context.Contracts.Update(contract);
}
