using GasApp.Domain.Entities;
using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository(AppDbContext context) : IRolePermissionRepository
{
    public async Task<IReadOnlyList<RolePermission>> GetAllAsync(CancellationToken ct = default)
        => await context.RolePermissions.ToListAsync(ct);

    public async Task<RolePermission?> GetAsync(UserRole role, AppPermission permission, CancellationToken ct = default)
        => await context.RolePermissions.FirstOrDefaultAsync(r => r.Role == role && r.Permission == permission, ct);

    public async Task AddAsync(RolePermission permission, CancellationToken ct = default)
        => await context.RolePermissions.AddAsync(permission, ct);

    public void Update(RolePermission permission)
        => context.RolePermissions.Update(permission);
}
