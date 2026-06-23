using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ToggleUserStatusCommand>
{
    public async Task Handle(ToggleUserStatusCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Users.User), cmd.Id);

        if (cmd.Activate) user.Activate();
        else user.Deactivate();

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
