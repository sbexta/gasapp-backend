using MediatR;

namespace GasApp.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password, string? DeviceInfo = null) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken, UserDto User);

public record UserDto(Guid Id, string Email, string FullName, string Role);
