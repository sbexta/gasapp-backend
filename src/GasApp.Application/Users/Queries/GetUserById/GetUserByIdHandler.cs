using GasApp.Application.Users.Commands.CreateUser;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Queries.GetUserById;

public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserResult>
{
    public async Task<UserResult> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Users.User), query.Id);

        return new UserResult(user.Id, user.Email.Value, user.FullName, user.Role.ToString(), user.IsActive);
    }
}
