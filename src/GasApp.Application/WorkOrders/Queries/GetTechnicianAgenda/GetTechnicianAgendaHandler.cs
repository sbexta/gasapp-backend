using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Queries.GetTechnicianAgenda;

public class GetTechnicianAgendaHandler(IWorkOrderRepository workOrderRepo, ILocationRepository locationRepo)
    : IRequestHandler<GetTechnicianAgendaQuery, IReadOnlyList<AgendaItemDto>>
{
    public async Task<IReadOnlyList<AgendaItemDto>> Handle(GetTechnicianAgendaQuery request, CancellationToken cancellationToken)
    {
        var workOrders = await workOrderRepo.GetTechnicianAgendaAsync(request.TechnicianId, request.Date, cancellationToken);

        var locationIds = workOrders.Select(w => w.LocationId).Distinct().ToList();
        var locations = new Dictionary<Guid, (string Name, string Address, string Municipality)>();

        foreach (var locationId in locationIds)
        {
            var loc = await locationRepo.GetByIdAsync(locationId, cancellationToken);
            if (loc != null)
                locations[locationId] = (loc.Name, loc.Address, loc.Municipality);
        }

        return workOrders.Select(w =>
        {
            locations.TryGetValue(w.LocationId, out var loc);
            return new AgendaItemDto(
                w.Id, w.OrderNumber, w.LocationId,
                loc.Name ?? "", loc.Address ?? "", loc.Municipality ?? "",
                w.ScheduledDate, w.Status.ToString());
        }).ToList();
    }
}
