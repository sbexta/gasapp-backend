using MediatR;

namespace GasApp.Application.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest;
