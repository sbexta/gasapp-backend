using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionHistory;

public record GetInspectionHistoryQuery(Guid InspectionId) : IRequest<IReadOnlyList<InspectionHistoryDto>>;

public record InspectionHistoryDto(
    Guid Id,
    string? PreviousStatus,
    string NewStatus,
    DateTime ChangedAt,
    Guid? ChangedById,
    string? ChangedByName,
    string? Notes);
