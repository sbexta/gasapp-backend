using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IInspectionStatusHistoryRepository
{
    Task<IReadOnlyList<InspectionStatusHistory>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task AddAsync(InspectionStatusHistory entry, CancellationToken ct = default);
}
