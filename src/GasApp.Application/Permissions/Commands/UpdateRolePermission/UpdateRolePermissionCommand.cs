using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Permissions.Commands.UpdateRolePermission;

public record UpdateRolePermissionCommand(UserRole Role, AppPermission Permission, bool IsGranted) : IRequest;
