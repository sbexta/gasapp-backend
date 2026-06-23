using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Enums;

namespace GasApp.Domain.Repositories;

public interface IWorkOrderRepository
{
    Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WorkOrder?> GetByNumberAsync(string orderNumber, CancellationToken ct = default);
    Task<(IReadOnlyList<WorkOrder> Items, int TotalCount)> GetPagedAsync(int page, int pageSize,
        WorkOrderStatus? status = null, Guid? technicianId = null,
        DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    Task<IReadOnlyList<WorkOrder>> GetTechnicianAgendaAsync(Guid technicianId, DateTime date, CancellationToken ct = default);
    Task AddAsync(WorkOrder workOrder, CancellationToken ct = default);
    void Update(WorkOrder workOrder);
}
