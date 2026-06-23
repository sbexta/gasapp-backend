using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Installations.Commands.CreateInstallation;

public class CreateInstallationHandler(
    IInstallationRepository installationRepo,
    ILocationRepository locationRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInstallationCommand, Guid>
{
    public async Task<Guid> Handle(CreateInstallationCommand request, CancellationToken cancellationToken)
    {
        var location = await locationRepo.GetByIdAsync(request.LocationId, cancellationToken)
            ?? throw new NotFoundException("Sede", request.LocationId);

        var installation = Installation.Create(request.LocationId, request.Name, request.Type,
            request.SerialNumber, request.Brand, request.Model, request.InstallationYear, request.Notes);

        await installationRepo.AddAsync(installation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return installation.Id;
    }
}
