using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Enums;

namespace GasApp.Domain.Repositories;

public interface IInspectionRepository
{
    Task<Inspection?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Inspection?> GetByWorkOrderIdAsync(Guid workOrderId, CancellationToken ct = default);
    Task<(IReadOnlyList<Inspection> Items, int Total)> GetPagedAsync(
        int page, int pageSize, InspectionStatus? status, CancellationToken ct = default);
    Task AddAsync(Inspection inspection, CancellationToken ct = default);
    void Update(Inspection inspection);
}
