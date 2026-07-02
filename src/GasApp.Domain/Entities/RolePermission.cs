using GasApp.Domain.Enums;

namespace GasApp.Domain.Entities;

public class RolePermission : AuditableEntity
{
    public UserRole Role { get; private set; }
    public AppPermission Permission { get; private set; }
    public bool IsGranted { get; private set; }

    private RolePermission() { }

    public static RolePermission Create(UserRole role, AppPermission permission, bool isGranted)
        => new() { Role = role, Permission = permission, IsGranted = isGranted };

    public void SetGranted(bool isGranted)
    {
        IsGranted = isGranted;
        Touch();
    }
}
