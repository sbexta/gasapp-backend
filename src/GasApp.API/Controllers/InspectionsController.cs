using GasApp.Application.Common.Interfaces;
using GasApp.Application.Inspections.Commands.AddFinding;
using GasApp.Application.Inspections.Commands.CaptureSignature;
using GasApp.Application.Inspections.Commands.SubmitChecklistResponse;
using GasApp.Application.Inspections.Commands.UploadEvidence;
using GasApp.Application.Inspections.Queries.GetChecklistByWorkOrder;
using GasApp.Application.Inspections.Queries.GetInspectionDetail;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/inspections")]
[Authorize]
public class InspectionsController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
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

public record UploadEvidenceRequest(
    EvidenceType Type, string FileName, string ContentType,
    long FileSizeBytes, string Base64Data, Guid? ChecklistItemId, string? Notes);
