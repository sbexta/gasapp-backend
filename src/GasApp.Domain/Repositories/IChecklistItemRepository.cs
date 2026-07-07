using GasApp.Domain.Entities.Checklists;

namespace GasApp.Domain.Repositories;

public interface IChecklistItemRepository
{
    Task<ChecklistItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ChecklistItem item, CancellationToken ct = default);
    void Update(ChecklistItem item);
    void Remove(ChecklistItem item);
}
