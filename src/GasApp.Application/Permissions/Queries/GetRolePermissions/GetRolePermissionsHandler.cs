using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Permissions.Queries.GetRolePermissions;

public class GetRolePermissionsHandler(IRolePermissionRepository repo)
    : IRequestHandler<GetRolePermissionsQuery, RolePermissionMatrixDto>
{
    private static readonly UserRole[] Roles =
        [UserRole.Admin, UserRole.Supervisor, UserRole.Technician, UserRole.ClientUser, UserRole.Auditor];

    private static readonly AppPermission[] Permissions =
        (AppPermission[])Enum.GetValues(typeof(AppPermission));

    public async Task<RolePermissionMatrixDto> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var stored = await repo.GetAllAsync(cancellationToken);
        var lookup = stored.ToDictionary(p => (p.Role, p.Permission), p => p.IsGranted);

        var matrix = new Dictionary<string, Dictionary<string, bool>>();

        foreach (var role in Roles)
        {
            matrix[role.ToString()] = Permissions.ToDictionary(
                p => p.ToString(),
                p => lookup.TryGetValue((role, p), out var granted) && granted
            );
        }

        return new RolePermissionMatrixDto(
            Roles.Select(r => r.ToString()).ToList(),
            Permissions.Select(p => p.ToString()).ToList(),
            matrix
        );
    }
}
