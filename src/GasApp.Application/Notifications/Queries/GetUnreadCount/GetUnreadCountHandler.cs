using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountHandler(INotificationRepository notifRepo, ICurrentUserService currentUser)
    : IRequestHandler<GetUnreadCountQuery, int>
{
    public Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        => notifRepo.GetUnreadCountAsync(currentUser.UserId, cancellationToken);
}
