using GasApp.Domain.Entities.Checklists;

namespace GasApp.Domain.Repositories;

public interface IChecklistItemRepository
{
    Task AddAsync(ChecklistItem item, CancellationToken ct = default);
}
