using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Auth.Commands.Login;

public class LoginHandler(
    IUserRepository userRepository,
    IUserSessionRepository sessionRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, LoginResult>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<LoginResult> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(cmd.Email.ToLowerInvariant(), ct)
            ?? throw new DomainException("Credenciales inválidas.");

        if (!user.IsActive)
            throw new DomainException("La cuenta está desactivada.");

        if (!passwordHasher.Verify(cmd.Password, user.HashedPassword))
            throw new DomainException("Credenciales inválidas.");

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var rawRefreshToken = jwtTokenService.GenerateRefreshToken();
        var tokenHash = jwtTokenService.HashToken(rawRefreshToken);

        var session = UserSession.Create(
            user.Id,
            tokenHash,
            DateTime.UtcNow.Add(RefreshTokenLifetime),
            cmd.DeviceInfo);

        await sessionRepository.AddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResult(
            accessToken,
            rawRefreshToken,
            new UserDto(user.Id, user.Email.Value, user.FullName, user.Role.ToString()));
    }
}
