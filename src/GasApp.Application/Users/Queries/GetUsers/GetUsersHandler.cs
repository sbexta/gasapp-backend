using GasApp.Application.Common.Models;
using GasApp.Application.Users.Commands.CreateUser;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Users.Queries.GetUsers;

public class GetUsersHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, PagedResult<UserResult>>
{
    public async Task<PagedResult<UserResult>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(ct);

        var filtered = users.AsEnumerable();

        if (query.Role.HasValue)
            filtered = filtered.Where(u => u.Role == query.Role.Value);

        if (query.IsActive.HasValue)
            filtered = filtered.Where(u => u.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLowerInvariant();
            filtered = filtered.Where(u =>
                u.FullName.ToLowerInvariant().Contains(search) ||
                u.Email.Value.Contains(search));
        }

        var totalList = filtered.ToList();
        var paged = totalList
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserResult(u.Id, u.Email.Value, u.FullName, u.Role.ToString(), u.IsActive))
            .ToList();

        return new PagedResult<UserResult>(paged, totalList.Count, query.Page, query.PageSize);
    }
}
