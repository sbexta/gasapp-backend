using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Commands.ChangePassword;

public class ChangePasswordHandler(IUserRepository repo, IPasswordHasher hasher, IUnitOfWork uow)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        if (!hasher.Verify(request.CurrentPassword, user.HashedPassword))
            throw new DomainException("La contraseña actual es incorrecta.");

        if (request.NewPassword.Length < 8)
            throw new DomainException("La nueva contraseña debe tener al menos 8 caracteres.");

        user.ChangePassword(hasher.Hash(request.NewPassword));
        repo.Update(user);
        await uow.SaveChangesAsync(ct);
    }
}
