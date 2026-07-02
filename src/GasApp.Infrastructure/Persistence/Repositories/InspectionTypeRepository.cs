using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class InspectionTypeRepository(AppDbContext context) : IInspectionTypeRepository
{
    public async Task<IReadOnlyList<InspectionType>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.InspectionTypes
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

    public async Task<InspectionType?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.InspectionTypes.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(InspectionType inspectionType, CancellationToken ct = default)
        => await context.InspectionTypes.AddAsync(inspectionType, ct);

    public void Update(InspectionType inspectionType)
        => context.InspectionTypes.Update(inspectionType);
}
