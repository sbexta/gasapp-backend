using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using GasApp.Domain.ValueObjects;
using MediatR;

namespace GasApp.Application.Locations.Commands.CreateLocation;

public class CreateLocationHandler(
    ILocationRepository locationRepo,
    IContractRepository contractRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLocationCommand, Guid>
{
    public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepo.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException("Contrato", request.ContractId);

        GeoPoint? coordinates = null;
        if (request.Latitude.HasValue && request.Longitude.HasValue)
            coordinates = new GeoPoint(request.Latitude.Value, request.Longitude.Value);

        var location = Location.Create(request.ContractId, request.Name, request.Address,
            request.Municipality, request.Department, coordinates);

        await locationRepo.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return location.Id;
    }
}
