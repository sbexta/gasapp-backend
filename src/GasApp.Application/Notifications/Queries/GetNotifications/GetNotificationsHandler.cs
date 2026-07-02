using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler(
    INotificationRepository notifRepo,
    ICurrentUserService currentUser)
    : IRequestHandler<GetNotificationsQuery, NotificationsResult>
{
    public async Task<NotificationsResult> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await notifRepo.GetByUserIdAsync(
            currentUser.UserId, request.Page, request.PageSize, cancellationToken);

        var unread = await notifRepo.GetUnreadCountAsync(currentUser.UserId, cancellationToken);

        return new NotificationsResult(
            items.Select(n => new NotificationDto(
                n.Id, n.Title, n.Body, n.Type, n.ReferenceId, n.IsRead, n.CreatedAt)).ToList(),
            total, unread);
    }
}
