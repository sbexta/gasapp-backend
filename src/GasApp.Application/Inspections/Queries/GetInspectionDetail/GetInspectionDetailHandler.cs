using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionDetail;

public class GetInspectionDetailHandler(
    IInspectionRepository inspectionRepo,
    IChecklistResponseRepository responseRepo,
    IFindingRepository findingRepo,
    IInspectionSignatureRepository signatureRepo)
    : IRequestHandler<GetInspectionDetailQuery, InspectionDetailDto>
{
    public async Task<InspectionDetailDto> Handle(GetInspectionDetailQuery request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var responses = await responseRepo.GetByInspectionIdAsync(request.InspectionId, cancellationToken);
        var findings = await findingRepo.GetByInspectionIdAsync(request.InspectionId, cancellationToken);
        var signature = await signatureRepo.GetByInspectionIdAsync(request.InspectionId, cancellationToken);

        return new InspectionDetailDto(
            inspection.Id,
            inspection.WorkOrderId,
            inspection.WorkOrder.OrderNumber,
            inspection.WorkOrder.ScheduledDate,
            inspection.Status.ToString(),
            inspection.StartedAt,
            inspection.CompletedAt,
            inspection.TechnicianNotes,
            responses.Select(r => new ChecklistResponseDto(
                r.Id, r.ChecklistItemId, r.TextValue, r.BoolValue, r.NumericValue, r.Complies, r.Notes
            )).ToList(),
            findings.Select(f => new FindingDto(
                f.Id, f.Description, f.Severity.ToString(), f.RequiresCorrection, f.IsResolved, f.CorrectiveAction, f.ChecklistItemId
            )).ToList(),
            signature is not null,
            signature is not null
                ? new SignatureDto(signature.SignerName, signature.SignerDocument, signature.SignedAt, signature.SignatureData)
                : null,
            inspection.LocationLat,
            inspection.LocationLng,
            inspection.LocationCapturedAt
        );
    }
}
