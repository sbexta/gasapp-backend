using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Inspections.Commands.AddFinding;

public record AddFindingCommand(
    Guid InspectionId,
    string Description,
    FindingSeverity Severity,
    bool RequiresCorrection,
    Guid? ChecklistItemId,
    string? CorrectiveAction
) : IRequest<Guid>;
