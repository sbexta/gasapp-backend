using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class InspectionStatusHistoryRepository(AppDbContext context) : IInspectionStatusHistoryRepository
{
    public async Task<IReadOnlyList<InspectionStatusHistory>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.InspectionStatusHistories
            .Where(h => h.InspectionId == inspectionId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync(ct);

    public async Task AddAsync(InspectionStatusHistory entry, CancellationToken ct = default)
        => await context.InspectionStatusHistories.AddAsync(entry, ct);
}
