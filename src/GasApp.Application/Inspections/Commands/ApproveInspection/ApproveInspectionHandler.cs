using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.ApproveInspection;

public class ApproveInspectionHandler(
    IInspectionRepository inspectionRepo,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveInspectionCommand>
{
    public async Task Handle(ApproveInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        // TechnicalReview → GeneratingDocs → Completed in one admin action
        inspection.TransitionStatus(InspectionStatus.GeneratingDocs, currentUser.Role, request.SupervisorNotes);
        inspection.TransitionStatus(InspectionStatus.Completed, UserRole.Admin);

        inspectionRepo.Update(inspection);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
