using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class InstallationRepository(AppDbContext context) : IInstallationRepository
{
    public async Task<Installation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Installations.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<Installation>> GetByLocationIdAsync(Guid locationId, CancellationToken ct = default)
        => await context.Installations
            .Where(i => i.LocationId == locationId)
            .OrderBy(i => i.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Installation installation, CancellationToken ct = default)
        => await context.Installations.AddAsync(installation, ct);

    public void Update(Installation installation)
        => context.Installations.Update(installation);
}
