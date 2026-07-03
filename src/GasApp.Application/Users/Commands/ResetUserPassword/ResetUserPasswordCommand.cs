using MediatR;

namespace GasApp.Application.Users.Commands.ResetUserPassword;

public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest;
