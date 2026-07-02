using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Repositories;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ChecklistItemRepository(AppDbContext context) : IChecklistItemRepository
{
    public async Task AddAsync(ChecklistItem item, CancellationToken ct = default)
        => await context.ChecklistItems.AddAsync(item, ct);
}
