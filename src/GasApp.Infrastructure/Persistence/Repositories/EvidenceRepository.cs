using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class EvidenceRepository(AppDbContext context) : IEvidenceRepository
{
    public async Task<Evidence?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Evidences.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Evidence>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.Evidences
            .Where(e => e.InspectionId == inspectionId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Evidence evidence, CancellationToken ct = default)
        => await context.Evidences.AddAsync(evidence, ct);
}
