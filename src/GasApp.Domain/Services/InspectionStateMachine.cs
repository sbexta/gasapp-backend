using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Services;

public static class InspectionStateMachine
{
    private static readonly Dictionary<InspectionStatus, InspectionStatus[]> AllowedTransitions = new()
    {
        [InspectionStatus.Pending]         = [InspectionStatus.PreCheck],
        [InspectionStatus.PreCheck]        = [InspectionStatus.InProgress, InspectionStatus.Pending],
        [InspectionStatus.InProgress]      = [InspectionStatus.TechnicalReview, InspectionStatus.RequiresFollowup],
        [InspectionStatus.TechnicalReview] = [InspectionStatus.GeneratingDocs, InspectionStatus.Rejected, InspectionStatus.RequiresFollowup],
        [InspectionStatus.GeneratingDocs]  = [InspectionStatus.Completed],
        [InspectionStatus.RequiresFollowup]= [InspectionStatus.InProgress, InspectionStatus.TechnicalReview],
    };

    // Roles autorizados por par (from, to)
    private static readonly Dictionary<(InspectionStatus From, InspectionStatus To), UserRole[]> RolePermissions = new()
    {
        [(InspectionStatus.Pending, InspectionStatus.PreCheck)]                  = [UserRole.Technician, UserRole.Supervisor],
        [(InspectionStatus.PreCheck, InspectionStatus.InProgress)]               = [UserRole.Technician, UserRole.Supervisor],
        [(InspectionStatus.PreCheck, InspectionStatus.Pending)]                  = [UserRole.Technician, UserRole.Supervisor],
        [(InspectionStatus.InProgress, InspectionStatus.TechnicalReview)]        = [UserRole.Technician],
        [(InspectionStatus.InProgress, InspectionStatus.RequiresFollowup)]       = [UserRole.Supervisor, UserRole.Admin],
        [(InspectionStatus.TechnicalReview, InspectionStatus.GeneratingDocs)]    = [UserRole.Supervisor, UserRole.Admin],
        [(InspectionStatus.TechnicalReview, InspectionStatus.Rejected)]          = [UserRole.Supervisor, UserRole.Admin],
        [(InspectionStatus.TechnicalReview, InspectionStatus.RequiresFollowup)]  = [UserRole.Supervisor, UserRole.Admin],
        [(InspectionStatus.RequiresFollowup, InspectionStatus.InProgress)]       = [UserRole.Technician, UserRole.Supervisor],
        [(InspectionStatus.RequiresFollowup, InspectionStatus.TechnicalReview)]  = [UserRole.Supervisor, UserRole.Admin],
        [(InspectionStatus.GeneratingDocs, InspectionStatus.Completed)]          = [UserRole.Admin],  // sistema/job
    };

    public static void ValidateTransition(InspectionStatus current, InspectionStatus target, UserRole role)
    {
        if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(target))
            throw new InvalidStateTransitionException(current.ToString(), target.ToString());

        var key = (current, target);
        if (RolePermissions.TryGetValue(key, out var allowedRoles) && !allowedRoles.Contains(role))
            throw new DomainException($"El rol '{role}' no está autorizado para realizar la transición de '{current}' a '{target}'.");
    }

    public static bool CanTransition(InspectionStatus current, InspectionStatus target) =>
        AllowedTransitions.TryGetValue(current, out var allowed) && allowed.Contains(target);

    public static IReadOnlyList<InspectionStatus> GetAllowedTransitions(InspectionStatus current) =>
        AllowedTransitions.TryGetValue(current, out var allowed) ? allowed : [];
}
