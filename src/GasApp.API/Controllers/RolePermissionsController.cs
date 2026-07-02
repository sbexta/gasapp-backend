using GasApp.Application.Permissions.Commands.UpdateRolePermission;
using GasApp.Application.Permissions.Queries.GetRolePermissions;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/role-permissions")]
[Authorize(Roles = "Admin")]
public class RolePermissionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatrix(CancellationToken ct)
    {
        var result = await mediator.Send(new GetRolePermissionsQuery(), ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePermissionRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, out var role))
            return BadRequest($"Rol inválido: {request.Role}");
        if (!Enum.TryParse<AppPermission>(request.Permission, out var permission))
            return BadRequest($"Permiso inválido: {request.Permission}");

        await mediator.Send(new UpdateRolePermissionCommand(role, permission, request.IsGranted), ct);
        return NoContent();
    }
}

public record UpdatePermissionRequest(string Role, string Permission, bool IsGranted);
