using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.ApproveInspection;

public class ApproveInspectionHandler(
    IInspectionRepository inspectionRepo,
    IWorkOrderRepository workOrderRepo,
    ICurrentUserService currentUser,
    ICertificateService certificateService,
    ICertificateRepository certRepo,
    IInspectionStatusHistoryRepository historyRepo,
    INotificationRepository notifRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveInspectionCommand>
{
    public async Task Handle(ApproveInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var prevStatus = inspection.Status;

        // TechnicalReview → GeneratingDocs
        inspection.TransitionStatus(InspectionStatus.GeneratingDocs, currentUser.Role, request.SupervisorNotes);
        await historyRepo.AddAsync(InspectionStatusHistory.Create(
            inspection.Id, prevStatus, InspectionStatus.GeneratingDocs, currentUser.UserId, request.SupervisorNotes), cancellationToken);

        // Generate PDF certificate
        var (certNumber, filePath) = await certificateService.GenerateAsync(inspection.Id, cancellationToken);
        var cert = InspectionCertificate.Create(inspection.Id, certNumber, filePath, currentUser.UserId);
        await certRepo.AddAsync(cert, cancellationToken);

        // GeneratingDocs → Completed
        inspection.TransitionStatus(InspectionStatus.Completed, UserRole.Admin);
        await historyRepo.AddAsync(InspectionStatusHistory.Create(
            inspection.Id, InspectionStatus.GeneratingDocs, InspectionStatus.Completed, currentUser.UserId), cancellationToken);

        // Complete the work order
        var workOrder = await workOrderRepo.GetByIdAsync(inspection.WorkOrderId, cancellationToken)
            ?? throw new NotFoundException("OrdenDeTrabajo", inspection.WorkOrderId);
        workOrder.Complete();
        workOrderRepo.Update(workOrder);

        // Notify technician
        var notif = Notification.Create(
            inspection.TechnicianId,
            "Inspección aprobada",
            $"Tu inspección fue aprobada. Certificado {certNumber} generado.",
            "InspectionApproved",
            inspection.Id);
        await notifRepo.AddAsync(notif, cancellationToken);

        inspectionRepo.Update(inspection);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
