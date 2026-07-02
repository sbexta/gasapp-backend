using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionDetail;

public record GetInspectionDetailQuery(Guid InspectionId) : IRequest<InspectionDetailDto>;

public record InspectionDetailDto(
    Guid Id,
    Guid WorkOrderId,
    string OrderNumber,
    DateTime ScheduledDate,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? TechnicianNotes,
    IReadOnlyList<ChecklistResponseDto> Responses,
    IReadOnlyList<FindingDto> Findings,
    bool HasSignature
);

public record ChecklistResponseDto(
    Guid Id,
    Guid ChecklistItemId,
    string? TextValue,
    bool? BoolValue,
    decimal? NumericValue,
    bool Complies,
    string? Notes
);

public record FindingDto(
    Guid Id,
    string Description,
    string Severity,
    bool RequiresCorrection,
    bool IsResolved,
    string? CorrectiveAction,
    Guid? ChecklistItemId
);
