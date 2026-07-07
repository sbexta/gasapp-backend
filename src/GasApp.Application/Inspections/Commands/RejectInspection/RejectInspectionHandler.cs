using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.RejectInspection;

public class RejectInspectionHandler(
    IInspectionRepository inspectionRepo,
    ICurrentUserService currentUser,
    IInspectionStatusHistoryRepository historyRepo,
    INotificationRepository notifRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RejectInspectionCommand>
{
    public async Task Handle(RejectInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var prevStatus = inspection.Status;

        inspection.TransitionStatus(InspectionStatus.Rejected, currentUser.Role, request.SupervisorNotes);

        await historyRepo.AddAsync(InspectionStatusHistory.Create(
            inspection.Id, prevStatus, InspectionStatus.Rejected, currentUser.UserId, request.SupervisorNotes), cancellationToken);

        var notif = Notification.Create(
            inspection.TechnicianId,
            "Inspección rechazada",
            $"Tu inspección fue rechazada por el supervisor.{(request.SupervisorNotes != null ? $" Motivo: {request.SupervisorNotes}" : "")}",
            "InspectionRejected",
            inspection.Id);
        await notifRepo.AddAsync(notif, cancellationToken);

        inspectionRepo.Update(inspection);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
