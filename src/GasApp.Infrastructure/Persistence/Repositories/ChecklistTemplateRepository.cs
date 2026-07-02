using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ChecklistTemplateRepository(AppDbContext context) : IChecklistTemplateRepository
{
    public async Task<ChecklistTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ChecklistTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<ChecklistTemplate?> GetByIdWithSectionsAsync(Guid id, CancellationToken ct = default)
        => await context.ChecklistTemplates
            .Include(t => t.Sections.OrderBy(s => s.Order))
                .ThenInclude(s => s.Items.OrderBy(i => i.Order))
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<ChecklistTemplate?> GetByInspectionTypeIdAsync(Guid inspectionTypeId, CancellationToken ct = default)
        => await context.ChecklistTemplates
            .Include(t => t.Sections.OrderBy(s => s.Order))
                .ThenInclude(s => s.Items.OrderBy(i => i.Order))
            .Where(t => t.IsActive && t.InspectionTypeId == inspectionTypeId)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ChecklistTemplate>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.ChecklistTemplates
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

    public async Task AddAsync(ChecklistTemplate template, CancellationToken ct = default)
        => await context.ChecklistTemplates.AddAsync(template, ct);

    public void Update(ChecklistTemplate template)
        => context.ChecklistTemplates.Update(template);
}
