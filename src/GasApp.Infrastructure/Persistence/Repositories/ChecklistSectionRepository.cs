using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ChecklistSectionRepository(AppDbContext context) : IChecklistSectionRepository
{
    public async Task<ChecklistSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ChecklistSections.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(ChecklistSection section, CancellationToken ct = default)
        => await context.ChecklistSections.AddAsync(section, ct);

    public void Update(ChecklistSection section)
        => context.ChecklistSections.Update(section);

    public void Remove(ChecklistSection section)
        => context.ChecklistSections.Remove(section);
}
