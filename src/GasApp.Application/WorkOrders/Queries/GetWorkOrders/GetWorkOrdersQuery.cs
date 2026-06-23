using GasApp.Application.Common.Models;
using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetWorkOrders;

public record GetWorkOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    WorkOrderStatus? Status = null,
    Guid? TechnicianId = null,
    DateTime? From = null,
    DateTime? To = null
) : IRequest<PagedResult<WorkOrderDto>>;

public record WorkOrderDto(
    Guid Id,
    string OrderNumber,
    Guid LocationId,
    Guid? AssignedTechnicianId,
    DateTime ScheduledDate,
    string Status,
    string? Notes
);
