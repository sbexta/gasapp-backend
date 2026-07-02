using GasApp.Domain.Entities;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Permissions.Commands.UpdateRolePermission;

public class UpdateRolePermissionHandler(
    IRolePermissionRepository repo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRolePermissionCommand>
{
    public async Task Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var existing = await repo.GetAsync(request.Role, request.Permission, cancellationToken);

        if (existing is null)
        {
            var newPerm = RolePermission.Create(request.Role, request.Permission, request.IsGranted);
            await repo.AddAsync(newPerm, cancellationToken);
        }
        else
        {
            existing.SetGranted(request.IsGranted);
            repo.Update(existing);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
