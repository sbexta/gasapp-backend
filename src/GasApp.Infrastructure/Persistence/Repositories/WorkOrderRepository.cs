using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class WorkOrderRepository(AppDbContext context) : IWorkOrderRepository
{
    public async Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.WorkOrders
            .Include(w => w.InspectionType)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<WorkOrder?> GetByNumberAsync(string orderNumber, CancellationToken ct = default)
        => await context.WorkOrders.FirstOrDefaultAsync(w => w.OrderNumber == orderNumber, ct);

    public async Task<(IReadOnlyList<WorkOrder> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, WorkOrderStatus? status = null, Guid? technicianId = null,
        DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var query = context.WorkOrders.Include(w => w.InspectionType).AsQueryable();

        if (status.HasValue)
            query = query.Where(w => w.Status == status.Value);

        if (technicianId.HasValue)
            query = query.Where(w => w.AssignedTechnicianId == technicianId.Value);

        if (from.HasValue)
            query = query.Where(w => w.ScheduledDate >= from.Value);

        if (to.HasValue)
            query = query.Where(w => w.ScheduledDate <= to.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(w => w.ScheduledDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<WorkOrder>> GetTechnicianAgendaAsync(
        Guid technicianId, DateTime date, CancellationToken ct = default)
        => await context.WorkOrders
            .Where(w => w.AssignedTechnicianId == technicianId
                && w.ScheduledDate.Date == date.Date
                && w.Status != WorkOrderStatus.Cancelled)
            .OrderBy(w => w.ScheduledDate)
            .ToListAsync(ct);

    public async Task AddAsync(WorkOrder workOrder, CancellationToken ct = default)
        => await context.WorkOrders.AddAsync(workOrder, ct);

    public void Update(WorkOrder workOrder)
        => context.WorkOrders.Update(workOrder);
}
