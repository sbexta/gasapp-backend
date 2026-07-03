using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Commands.ResetUserPassword;

public class ResetUserPasswordHandler(IUserRepository repo, IPasswordHasher hasher, IUnitOfWork uow)
    : IRequestHandler<ResetUserPasswordCommand>
{
    public async Task Handle(ResetUserPasswordCommand request, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        if (request.NewPassword.Length < 8)
            throw new DomainException("La nueva contraseña debe tener al menos 8 caracteres.");

        user.ChangePassword(hasher.Hash(request.NewPassword));
        repo.Update(user);
        await uow.SaveChangesAsync(ct);
    }
}
