using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetTechnicianAgenda;

public record GetTechnicianAgendaQuery(Guid TechnicianId, DateTime Date) : IRequest<IReadOnlyList<AgendaItemDto>>;

public record AgendaItemDto(
    Guid WorkOrderId,
    string OrderNumber,
    Guid LocationId,
    string LocationName,
    string LocationAddress,
    string Municipality,
    DateTime ScheduledDate,
    string Status
);
