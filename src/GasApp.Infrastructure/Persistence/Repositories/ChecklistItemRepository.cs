using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Repositories;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ChecklistItemRepository(AppDbContext context) : IChecklistItemRepository
{
    public async Task<ChecklistItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ChecklistItems.FindAsync([id], ct);

    public async Task AddAsync(ChecklistItem item, CancellationToken ct = default)
        => await context.ChecklistItems.AddAsync(item, ct);

    public void Update(ChecklistItem item)
        => context.ChecklistItems.Update(item);

    public void Remove(ChecklistItem item)
        => context.ChecklistItems.Remove(item);
}
