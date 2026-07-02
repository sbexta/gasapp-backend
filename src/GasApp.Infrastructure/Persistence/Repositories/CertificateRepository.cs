using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class CertificateRepository(AppDbContext context) : ICertificateRepository
{
    public async Task<InspectionCertificate?> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default)
        => await context.InspectionCertificates.FirstOrDefaultAsync(c => c.InspectionId == inspectionId, ct);

    public async Task AddAsync(InspectionCertificate cert, CancellationToken ct = default)
        => await context.InspectionCertificates.AddAsync(cert, ct);
}
