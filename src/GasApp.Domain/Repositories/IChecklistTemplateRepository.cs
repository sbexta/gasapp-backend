using GasApp.Domain.Entities.Checklists;

namespace GasApp.Domain.Repositories;

public interface IChecklistTemplateRepository
{
    Task<ChecklistTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ChecklistTemplate?> GetByIdWithSectionsAsync(Guid id, CancellationToken ct = default);
    Task<ChecklistTemplate?> GetByInspectionTypeIdAsync(Guid inspectionTypeId, CancellationToken ct = default);
    Task<IReadOnlyList<ChecklistTemplate>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(ChecklistTemplate template, CancellationToken ct = default);
    void Update(ChecklistTemplate template);
}
