using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IChecklistResponseRepository
{
    Task<ChecklistResponse?> GetByInspectionAndItemAsync(Guid inspectionId, Guid checklistItemId, CancellationToken ct = default);
    Task<IReadOnlyList<ChecklistResponse>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task AddAsync(ChecklistResponse response, CancellationToken ct = default);
    void Update(ChecklistResponse response);
}
