using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Client?> GetByNitAsync(string nit, CancellationToken ct = default)
        => await context.Clients.FirstOrDefaultAsync(c => c.Nit == nit, ct);

    public async Task<(IReadOnlyList<Client> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search = null, bool? isActive = null, CancellationToken ct = default)
    {
        var query = context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.BusinessName.Contains(search) || c.Nit.Contains(search));

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(c => c.BusinessName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Client client, CancellationToken ct = default)
        => await context.Clients.AddAsync(client, ct);

    public void Update(Client client)
        => context.Clients.Update(client);
}
