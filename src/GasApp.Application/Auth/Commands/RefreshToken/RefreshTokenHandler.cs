using GasApp.Application.Auth.Commands.Login;
using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Auth.Commands.RefreshToken;

public class RefreshTokenHandler(
    IUserSessionRepository sessionRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<LoginResult> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var tokenHash = jwtTokenService.HashToken(cmd.RefreshToken);
        var session = await sessionRepository.GetByRefreshTokenHashAsync(tokenHash, ct)
            ?? throw new DomainException("Refresh token inválido.");

        if (!session.IsValid)
            throw new DomainException("Refresh token expirado o revocado.");

        var user = await userRepository.GetByIdAsync(session.UserId, ct)
            ?? throw new DomainException("Usuario no encontrado.");

        if (!user.IsActive)
            throw new DomainException("La cuenta está desactivada.");

        session.Revoke();
        sessionRepository.Update(session);

        var newRawToken = jwtTokenService.GenerateRefreshToken();
        var newTokenHash = jwtTokenService.HashToken(newRawToken);
        var newSession = UserSession.Create(user.Id, newTokenHash, DateTime.UtcNow.Add(RefreshTokenLifetime), session.DeviceInfo);

        await sessionRepository.AddAsync(newSession, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResult(
            jwtTokenService.GenerateAccessToken(user),
            newRawToken,
            new UserDto(user.Id, user.Email.Value, user.FullName, user.Role.ToString()));
    }
}
