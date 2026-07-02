using GasApp.Domain.Entities;
using GasApp.Domain.Enums;

namespace GasApp.Domain.Repositories;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<RolePermission>> GetAllAsync(CancellationToken ct = default);
    Task<RolePermission?> GetAsync(UserRole role, AppPermission permission, CancellationToken ct = default);
    Task AddAsync(RolePermission permission, CancellationToken ct = default);
    void Update(RolePermission permission);
}
