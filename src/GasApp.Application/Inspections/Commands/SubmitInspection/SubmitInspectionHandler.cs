using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.SubmitInspection;

public class SubmitInspectionHandler(
    IInspectionRepository inspectionRepo,
    ICurrentUserService currentUser,
    IInspectionStatusHistoryRepository historyRepo,
    INotificationRepository notifRepo,
    IUserRepository userRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitInspectionCommand>
{
    public async Task Handle(SubmitInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        if (currentUser.Role == UserRole.Technician && inspection.TechnicianId != currentUser.UserId)
            throw new DomainException("Solo el técnico asignado puede enviar esta inspección.");

        var prevStatus = inspection.Status;
        inspection.TransitionStatus(InspectionStatus.TechnicalReview, currentUser.Role, request.TechnicianNotes);

        await historyRepo.AddAsync(InspectionStatusHistory.Create(
            inspection.Id, prevStatus, InspectionStatus.TechnicalReview,
            currentUser.UserId, request.TechnicianNotes), cancellationToken);

        // Notify all supervisors
        var supervisors = await userRepo.GetByRoleAsync(UserRole.Supervisor, cancellationToken);
        var notifs = supervisors.Select(s => Notification.Create(
            s.Id,
            "Inspección enviada para revisión",
            $"La inspección de la orden {inspection.WorkOrder?.OrderNumber ?? inspection.WorkOrderId.ToString()} requiere tu revisión.",
            "InspectionSubmitted",
            inspection.Id)).ToList();

        if (notifs.Count > 0)
            await notifRepo.AddRangeAsync(notifs, cancellationToken);

        inspectionRepo.Update(inspection);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
