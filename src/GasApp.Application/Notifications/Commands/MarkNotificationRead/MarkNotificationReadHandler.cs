using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadHandler(
    INotificationRepository notifRepo,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationReadCommand>
{
    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notif = await notifRepo.GetByIdAsync(request.NotificationId, cancellationToken)
            ?? throw new NotFoundException("Notificación", request.NotificationId);

        if (notif.UserId != currentUser.UserId)
            throw new DomainException("No tienes permiso para marcar esta notificación.");

        notif.MarkAsRead();
        notifRepo.Update(notif);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
