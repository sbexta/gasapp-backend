using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetWorkOrderById;

public record GetWorkOrderByIdQuery(Guid Id) : IRequest<WorkOrderDetailDto>;

public record WorkOrderDetailDto(
    Guid Id,
    string OrderNumber,
    string Status,
    DateTime ScheduledDate,
    string? Notes,
    WorkOrderLocationDto Location,
    WorkOrderClientDto Client
);

public record WorkOrderLocationDto(
    string Name,
    string Address,
    string Municipality,
    string Department
);

public record WorkOrderClientDto(
    string BusinessName,
    string? ContactPhone
);
