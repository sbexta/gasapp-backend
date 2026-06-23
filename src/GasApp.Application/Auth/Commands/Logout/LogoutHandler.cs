using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Auth.Commands.Logout;

public class LogoutHandler(
    IUserSessionRepository sessionRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand cmd, CancellationToken ct)
    {
        var tokenHash = jwtTokenService.HashToken(cmd.RefreshToken);
        var session = await sessionRepository.GetByRefreshTokenHashAsync(tokenHash, ct)
            ?? throw new DomainException("Sesión no encontrada.");

        if (session.IsRevoked)
            return;

        session.Revoke();
        sessionRepository.Update(session);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
