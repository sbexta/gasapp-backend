using GasApp.Application.Auth.Commands.Login;
using MediatR;

namespace GasApp.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResult>;
