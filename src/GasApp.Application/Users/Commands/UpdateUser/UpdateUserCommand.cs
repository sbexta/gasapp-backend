using MediatR;

namespace GasApp.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string? Phone) : IRequest;
