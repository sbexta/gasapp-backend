using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ChecklistResponseRepository(AppDbContext context) : IChecklistResponseRepository
{
    public async Task<ChecklistResponse?> GetByInspectionAndItemAsync(Guid inspectionId, Guid checklistItemId, CancellationToken ct = default)
        => await context.ChecklistResponses
            .FirstOrDefaultAsync(r => r.InspectionId == inspectionId && r.ChecklistItemId == checklistItemId, ct);

    public async Task<IReadOnlyList<ChecklistResponse>> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.ChecklistResponses
            .Where(r => r.InspectionId == inspectionId)
            .ToListAsync(ct);

    public async Task AddAsync(ChecklistResponse response, CancellationToken ct = default)
        => await context.ChecklistResponses.AddAsync(response, ct);

    public void Update(ChecklistResponse response)
        => context.ChecklistResponses.Update(response);
}
