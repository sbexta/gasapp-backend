using GasApp.Application.Reports.Queries.GetInspectionReport;
using GasApp.Application.Reports.Queries.GetKpis;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Roles = "Admin,Supervisor")]
public class ReportsController(IMediator mediator) : ControllerBase
{
    [HttpGet("inspections")]
    public async Task<IActionResult> GetInspections(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] string? status, [FromQuery] Guid? technicianId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        InspectionStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<InspectionStatus>(status, out var s))
            parsedStatus = s;

        var result = await mediator.Send(
            new GetInspectionReportQuery(from, to, parsedStatus, technicianId, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await mediator.Send(new GetKpisQuery(from, to), ct);
        return Ok(result);
    }

    [HttpGet("inspections/export")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] string? status, [FromQuery] Guid? technicianId,
        CancellationToken ct)
    {
        InspectionStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<InspectionStatus>(status, out var s))
            parsedStatus = s;

        var result = await mediator.Send(
            new GetInspectionReportQuery(from, to, parsedStatus, technicianId, 1, 10_000), ct);

        var sb = new StringBuilder();
        sb.AppendLine("Orden,Estado,Fecha Programada,Fecha Completada,Técnico,Cliente,Sede,Hallazgos,Certificado");

        foreach (var row in result.Items)
        {
            sb.AppendLine(string.Join(",", new[]
            {
                row.OrderNumber, row.Status,
                row.ScheduledDate.ToString("dd/MM/yyyy"),
                row.CompletedAt?.ToString("dd/MM/yyyy") ?? "",
                $"\"{row.TechnicianName}\"", $"\"{row.ClientName}\"", $"\"{row.LocationName}\"",
                row.FindingsCount.ToString(),
                row.HasCertificate ? "Sí" : "No"
            }));
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv; charset=utf-8", $"reporte-inspecciones-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
