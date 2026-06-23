using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Users.User), cmd.Id);

        user.UpdateProfile(cmd.FirstName, cmd.LastName, cmd.Phone);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
