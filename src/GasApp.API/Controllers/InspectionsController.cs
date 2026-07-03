using GasApp.Application.Common.Interfaces;
using GasApp.Application.Inspections.Commands.AddFinding;
using GasApp.Application.Inspections.Queries.GetInspectionHistory;
using GasApp.Domain.Repositories;
using GasApp.Application.Inspections.Commands.ApproveInspection;
using GasApp.Application.Inspections.Commands.CaptureSignature;
using GasApp.Application.Inspections.Commands.SubmitChecklistResponse;
using GasApp.Application.Inspections.Commands.SubmitInspection;
using GasApp.Application.Inspections.Commands.UploadEvidence;
using GasApp.Application.Inspections.Queries.GetChecklistByWorkOrder;
using GasApp.Application.Inspections.Queries.GetInspectionDetail;
using GasApp.Application.Inspections.Queries.GetInspections;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/inspections")]
[Authorize]
public class InspectionsController(
    IMediator mediator,
    ICurrentUserService currentUser,
    ICertificateRepository certRepo) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetInspectionsQuery(page, pageSize, status), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetInspectionDetailQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("by-work-order/{workOrderId:guid}/checklist")]
    public async Task<IActionResult> GetChecklist(Guid workOrderId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetChecklistByWorkOrderQuery(workOrderId), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost("{id:guid}/responses")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> SubmitResponse(Guid id, [FromBody] SubmitResponseRequest request, CancellationToken ct)
    {
        await mediator.Send(new SubmitChecklistResponseCommand(
            id, request.ChecklistItemId, request.TextValue, request.BoolValue,
            request.NumericValue, request.Complies, request.Notes), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/findings")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> AddFinding(Guid id, [FromBody] AddFindingRequest request, CancellationToken ct)
    {
        var findingId = await mediator.Send(new AddFindingCommand(
            id, request.Description, request.Severity,
            request.RequiresCorrection, request.ChecklistItemId, request.CorrectiveAction), ct);
        return Ok(new { id = findingId });
    }

    [HttpPost("{id:guid}/signature")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> CaptureSignature(Guid id, [FromBody] CaptureSignatureRequest request, CancellationToken ct)
    {
        await mediator.Send(new CaptureSignatureCommand(
            id, request.SignerName, request.SignatureData, request.SignerDocument), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitInspectionRequest request, CancellationToken ct)
    {
        await mediator.Send(new SubmitInspectionCommand(id, request.TechnicianNotes), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveInspectionRequest request, CancellationToken ct)
    {
        await mediator.Send(new ApproveInspectionCommand(id, request.SupervisorNotes), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetInspectionHistoryQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/certificate")]
    public async Task<IActionResult> DownloadCertificate(Guid id, CancellationToken ct)
    {
        var cert = await certRepo.GetByInspectionIdAsync(id, ct);
        if (cert == null) return NotFound(new { message = "Certificado no generado aún." });
        if (cert.PdfData.Length == 0) return NotFound(new { message = "Datos del certificado no disponibles." });

        return File(cert.PdfData, "application/pdf", $"{cert.CertificateNumber}.pdf");
    }

    [HttpPost("{id:guid}/evidences")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> UploadEvidence(Guid id, [FromBody] UploadEvidenceRequest request, CancellationToken ct)
    {
        var evidenceId = await mediator.Send(new UploadEvidenceCommand(
            id, request.Type, request.FileName, request.ContentType,
            request.FileSizeBytes, request.Base64Data, currentUser.UserId,
            request.ChecklistItemId, request.Notes), ct);
        return Ok(new { id = evidenceId });
    }
}

public record SubmitResponseRequest(
    Guid ChecklistItemId, string? TextValue, bool? BoolValue,
    decimal? NumericValue, bool Complies, string? Notes);

public record AddFindingRequest(
    string Description, FindingSeverity Severity, bool RequiresCorrection,
    Guid? ChecklistItemId, string? CorrectiveAction);

public record CaptureSignatureRequest(
    string SignerName, string SignatureData, string? SignerDocument);

public record SubmitInspectionRequest(string? TechnicianNotes);
public record ApproveInspectionRequest(string? SupervisorNotes);

public record UploadEvidenceRequest(
    EvidenceType Type, string FileName, string ContentType,
    long FileSizeBytes, string Base64Data, Guid? ChecklistItemId, string? Notes);
