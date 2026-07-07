using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface ICertificateRepository
{
    Task<InspectionCertificate?> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task<InspectionCertificate?> GetByPublicTokenAsync(Guid publicToken, CancellationToken ct = default);
    Task AddAsync(InspectionCertificate cert, CancellationToken ct = default);
}
