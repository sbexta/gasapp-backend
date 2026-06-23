using GasApp.Application.Users.Commands.CreateUser;
using MediatR;

namespace GasApp.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResult>;
