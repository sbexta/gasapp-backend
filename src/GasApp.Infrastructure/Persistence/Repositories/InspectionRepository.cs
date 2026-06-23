using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class InspectionRepository(AppDbContext context) : IInspectionRepository
{
    public async Task<Inspection?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Inspections
            .Include(i => i.WorkOrder)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Inspection?> GetByWorkOrderIdAsync(Guid workOrderId, CancellationToken ct = default)
        => await context.Inspections.FirstOrDefaultAsync(i => i.WorkOrderId == workOrderId, ct);

    public async Task AddAsync(Inspection inspection, CancellationToken ct = default)
        => await context.Inspections.AddAsync(inspection, ct);

    public void Update(Inspection inspection)
        => context.Inspections.Update(inspection);
}
