using GasApp.Domain.Entities.Checklists;

namespace GasApp.Domain.Repositories;

public interface IChecklistSectionRepository
{
    Task<ChecklistSection?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ChecklistSection section, CancellationToken ct = default);
}
