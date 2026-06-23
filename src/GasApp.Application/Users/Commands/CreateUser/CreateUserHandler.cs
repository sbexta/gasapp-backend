using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using GasApp.Domain.ValueObjects;
using MediatR;

namespace GasApp.Application.Users.Commands.CreateUser;

public class CreateUserHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, UserResult>
{
    public async Task<UserResult> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        var email = new Email(cmd.Email);

        var existing = await userRepository.GetByEmailAsync(email.Value, ct);
        if (existing != null)
            throw new DomainException($"Ya existe un usuario con el correo '{email.Value}'.");

        var hashedPassword = passwordHasher.Hash(cmd.Password);
        var user = User.Create(email, hashedPassword, cmd.FirstName, cmd.LastName, cmd.Role, cmd.Phone);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new UserResult(user.Id, user.Email.Value, user.FullName, user.Role.ToString(), user.IsActive);
    }
}
