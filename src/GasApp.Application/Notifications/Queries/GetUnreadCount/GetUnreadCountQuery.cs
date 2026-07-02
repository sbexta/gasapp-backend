using MediatR;

namespace GasApp.Application.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<int>;
