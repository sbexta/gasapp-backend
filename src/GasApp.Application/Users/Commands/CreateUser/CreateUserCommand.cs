using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role,
    string? Phone = null) : IRequest<UserResult>;

public record UserResult(Guid Id, string Email, string FullName, string Role, bool IsActive);
