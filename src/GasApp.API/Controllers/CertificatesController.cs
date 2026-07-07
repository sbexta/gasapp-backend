using GasApp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/certificates")]
public class CertificatesController(ICertificateRepository certRepo) : ControllerBase
{
    [HttpGet("public/{token:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadPublic(Guid token, CancellationToken ct)
    {
        var cert = await certRepo.GetByPublicTokenAsync(token, ct);
        if (cert == null || cert.PdfData.Length == 0)
            return NotFound(new { message = "Certificado no encontrado o no disponible." });

        return File(cert.PdfData, "application/pdf", $"{cert.CertificateNumber}.pdf");
    }
}
