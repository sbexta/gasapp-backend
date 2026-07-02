using MediatR;

namespace GasApp.Application.Locations.Queries.GetLocations;

public record LocationResult(Guid Id, string Name, string Address, string Municipality, string Department, string ClientName);

public record GetLocationsQuery : IRequest<IReadOnlyList<LocationResult>>;
