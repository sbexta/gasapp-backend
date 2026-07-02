using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class InspectionSignature : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public string SignerName { get; private set; } = null!;
    public string? SignerDocument { get; private set; }
    public string SignatureData { get; private set; } = null!; // base64 PNG
    public DateTime SignedAt { get; private set; }

    private InspectionSignature() { }

    public static InspectionSignature Create(Guid inspectionId, string signerName,
        string signatureData, string? signerDocument = null)
    {
        if (string.IsNullOrWhiteSpace(signerName))
            throw new DomainException("El nombre del firmante no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(signatureData))
            throw new DomainException("Los datos de la firma no pueden estar vacíos.");

        return new InspectionSignature
        {
            InspectionId = inspectionId,
            SignerName = signerName.Trim(),
            SignerDocument = signerDocument?.Trim(),
            SignatureData = signatureData,
            SignedAt = DateTime.UtcNow
        };
    }
}
