using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class FindingRepository(AppDbContext context) : IFindingRepository
{
    public async Task<Finding?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Findings.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<Finding>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.Findings
            .Where(f => f.InspectionId == inspectionId)
            .OrderByDescending(f => f.Severity)
            .ToListAsync(ct);

    public async Task AddAsync(Finding finding, CancellationToken ct = default)
        => await context.Findings.AddAsync(finding, ct);

    public void Update(Finding finding)
        => context.Findings.Update(finding);
}
