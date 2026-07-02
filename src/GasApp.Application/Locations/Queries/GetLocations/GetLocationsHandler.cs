using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Locations.Queries.GetLocations;

public class GetLocationsHandler(ILocationRepository locationRepository)
    : IRequestHandler<GetLocationsQuery, IReadOnlyList<LocationResult>>
{
    public async Task<IReadOnlyList<LocationResult>> Handle(GetLocationsQuery request, CancellationToken ct)
    {
        var locations = await locationRepository.GetAllActiveAsync(ct);
        return locations
            .Select(l => new LocationResult(
                l.Id,
                l.Name,
                l.Address,
                l.Municipality,
                l.Department,
                l.Contract?.Client?.BusinessName ?? string.Empty))
            .ToList();
    }
}
