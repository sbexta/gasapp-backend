using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class LocationRepository(AppDbContext context) : ILocationRepository
{
    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Locations.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Location?> GetByIdWithClientAsync(Guid id, CancellationToken ct = default)
        => await context.Locations
            .Include(l => l.Contract)
                .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IReadOnlyList<Location>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.Locations
            .Include(l => l.Contract)
                .ThenInclude(c => c.Client)
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Location>> GetByContractIdAsync(Guid contractId, CancellationToken ct = default)
        => await context.Locations
            .Where(l => l.ContractId == contractId)
            .OrderBy(l => l.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Location location, CancellationToken ct = default)
        => await context.Locations.AddAsync(location, ct);

    public void Update(Location location)
        => context.Locations.Update(location);
}
