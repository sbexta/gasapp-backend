using GasApp.Domain.Entities.Inspections;

namespace GasApp.Domain.Repositories;

public interface IInspectionSignatureRepository
{
    Task<InspectionSignature?> GetByInspectionIdAsync(Guid inspectionId, CancellationToken ct = default);
    Task AddAsync(InspectionSignature signature, CancellationToken ct = default);
}
