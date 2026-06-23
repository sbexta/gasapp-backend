using MediatR;

namespace GasApp.Application.Users.Commands.ToggleUserStatus;

public record ToggleUserStatusCommand(Guid Id, bool Activate) : IRequest;
