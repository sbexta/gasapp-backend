namespace GasApp.Application.Common.Interfaces;

public interface ICertificateService
{
    Task<(string CertificateNumber, byte[] PdfBytes)> GenerateAsync(Guid inspectionId, CancellationToken ct = default);
}
