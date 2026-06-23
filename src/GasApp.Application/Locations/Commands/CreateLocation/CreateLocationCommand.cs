using MediatR;

namespace GasApp.Application.Locations.Commands.CreateLocation;

public record CreateLocationCommand(
    Guid ContractId,
    string Name,
    string Address,
    string Municipality,
    string Department,
    double? Latitude,
    double? Longitude
) : IRequest<Guid>;
