using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.CaptureSignature;

public class CaptureSignatureHandler(
    IInspectionSignatureRepository signatureRepo,
    IInspectionRepository inspectionRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CaptureSignatureCommand>
{
    public async Task Handle(CaptureSignatureCommand request, CancellationToken cancellationToken)
    {
        _ = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var existing = await signatureRepo.GetByInspectionIdAsync(request.InspectionId, cancellationToken);
        if (existing is not null)
            throw new DomainException("Esta inspección ya tiene una firma registrada.");

        var signature = InspectionSignature.Create(
            request.InspectionId, request.SignerName,
            request.SignatureData, request.SignerDocument);

        await signatureRepo.AddAsync(signature, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
