using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IEvidenceRepository
{
    Task<Evidence?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Evidence>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task AddAsync(Evidence evidence, CancellationToken ct = default);
}
