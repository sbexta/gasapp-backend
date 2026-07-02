using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.UploadEvidence;

public class UploadEvidenceHandler(
    IEvidenceRepository evidenceRepo,
    IInspectionRepository inspectionRepo,
    IFileStorageService fileStorage,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UploadEvidenceCommand, Guid>
{
    public async Task<Guid> Handle(UploadEvidenceCommand request, CancellationToken cancellationToken)
    {
        _ = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var bytes = Convert.FromBase64String(request.Base64Data);
        var storagePath = await fileStorage.SaveAsync(
            request.InspectionId, request.FileName, bytes, cancellationToken);

        var evidence = Evidence.Create(
            request.InspectionId, request.Type, request.FileName,
            request.ContentType, request.FileSizeBytes, storagePath,
            request.UploadedBy, request.ChecklistItemId, request.Notes);

        await evidenceRepo.AddAsync(evidence, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return evidence.Id;
    }
}
