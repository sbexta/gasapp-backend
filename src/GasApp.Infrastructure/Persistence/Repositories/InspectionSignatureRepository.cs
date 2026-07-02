using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class InspectionSignatureRepository(AppDbContext context) : IInspectionSignatureRepository
{
    public async Task<InspectionSignature?> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.InspectionSignatures
            .FirstOrDefaultAsync(s => s.InspectionId == inspectionId, ct);

    public async Task AddAsync(InspectionSignature signature, CancellationToken ct = default)
        => await context.InspectionSignatures.AddAsync(signature, ct);
}
