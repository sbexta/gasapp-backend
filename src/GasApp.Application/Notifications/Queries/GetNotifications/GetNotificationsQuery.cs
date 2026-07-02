using MediatR;

namespace GasApp.Application.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(int Page = 1, int PageSize = 20) : IRequest<NotificationsResult>;

public record NotificationDto(
    Guid Id, string Title, string Body, string Type,
    Guid? ReferenceId, bool IsRead, DateTime CreatedAt);

public record NotificationsResult(IReadOnlyList<NotificationDto> Items, int Total, int UnreadCount);
