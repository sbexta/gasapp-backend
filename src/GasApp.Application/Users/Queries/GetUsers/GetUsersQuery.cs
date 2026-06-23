using GasApp.Application.Common.Models;
using GasApp.Application.Users.Commands.CreateUser;
using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    UserRole? Role = null,
    bool? IsActive = null,
    string? Search = null) : IRequest<PagedResult<UserResult>>;
