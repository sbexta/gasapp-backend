using GasApp.Application.Common.Models;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspections;

public record GetInspectionsQuery(int Page, int PageSize, string? Status) : IRequest<PagedResult<InspectionListDto>>;

public record InspectionListDto(
    Guid Id,
    Guid WorkOrderId,
    string OrderNumber,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime ScheduledDate
);
