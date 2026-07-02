namespace GasApp.Domain.Entities.Inspections;

public class InspectionCertificate : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public string CertificateNumber { get; private set; } = null!;
    public DateTime IssuedAt { get; private set; }
    public string FilePath { get; private set; } = null!;
    public Guid IssuedById { get; private set; }

    private InspectionCertificate() { }

    public static InspectionCertificate Create(Guid inspectionId, string number, string filePath, Guid issuedById)
        => new()
        {
            InspectionId = inspectionId,
            CertificateNumber = number,
            FilePath = filePath,
            IssuedAt = DateTime.UtcNow,
            IssuedById = issuedById
        };
}
