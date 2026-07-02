using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IInspectionTypeRepository
{
    Task<IReadOnlyList<InspectionType>> GetAllActiveAsync(CancellationToken ct = default);
    Task<InspectionType?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(InspectionType inspectionType, CancellationToken ct = default);
    void Update(InspectionType inspectionType);
}
