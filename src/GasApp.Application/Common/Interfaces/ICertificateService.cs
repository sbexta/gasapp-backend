namespace GasApp.Application.Common.Interfaces;

public interface ICertificateService
{
    /// <summary>Generates a PDF certificate, saves it to disk, and returns (certNumber, filePath).</summary>
    Task<(string CertificateNumber, string FilePath)> GenerateAsync(Guid inspectionId, CancellationToken ct = default);
}
