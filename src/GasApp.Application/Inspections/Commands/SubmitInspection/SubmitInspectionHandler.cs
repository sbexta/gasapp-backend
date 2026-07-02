using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.SubmitInspection;

public class SubmitInspectionHandler(
    IInspectionRepository inspectionRepo,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitInspectionCommand>
{
    public async Task Handle(SubmitInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        if (currentUser.Role == UserRole.Technician && inspection.TechnicianId != currentUser.UserId)
            throw new DomainException("Solo el técnico asignado puede enviar esta inspección.");

        inspection.TransitionStatus(InspectionStatus.TechnicalReview, currentUser.Role, request.TechnicianNotes);

        inspectionRepo.Update(inspection);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
