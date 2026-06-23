using MediatR;

namespace GasApp.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
