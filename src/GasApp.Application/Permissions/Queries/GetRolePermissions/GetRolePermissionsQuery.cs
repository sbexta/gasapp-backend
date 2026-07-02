using MediatR;

namespace GasApp.Application.Permissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery : IRequest<RolePermissionMatrixDto>;

public record RolePermissionMatrixDto(
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    // [role][permission] = isGranted
    Dictionary<string, Dictionary<string, bool>> Matrix
);
