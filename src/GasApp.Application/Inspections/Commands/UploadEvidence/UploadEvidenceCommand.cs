using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Inspections.Commands.UploadEvidence;

public record UploadEvidenceCommand(
    Guid InspectionId,
    EvidenceType Type,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string Base64Data,
    Guid UploadedBy,
    Guid? ChecklistItemId,
    string? Notes
) : IRequest<Guid>;
