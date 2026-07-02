using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IFindingRepository
{
    Task<Finding?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Finding>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task AddAsync(Finding finding, CancellationToken ct = default);
    void Update(Finding finding);
}
