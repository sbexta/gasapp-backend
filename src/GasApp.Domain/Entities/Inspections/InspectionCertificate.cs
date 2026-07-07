namespace GasApp.Domain.Entities.Inspections;

public class InspectionCertificate : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public string CertificateNumber { get; private set; } = null!;
    public DateTime IssuedAt { get; private set; }
    public string? FilePath { get; private set; }
    public byte[] PdfData { get; private set; } = Array.Empty<byte>();
    public Guid IssuedById { get; private set; }
    public Guid PublicToken { get; private set; }

    private InspectionCertificate() { }

    public static InspectionCertificate Create(Guid inspectionId, string number, byte[] pdfData, Guid issuedById)
        => new()
        {
            InspectionId = inspectionId,
            CertificateNumber = number,
            PdfData = pdfData,
            IssuedAt = DateTime.UtcNow,
            IssuedById = issuedById,
            PublicToken = Guid.NewGuid()
        };
}
