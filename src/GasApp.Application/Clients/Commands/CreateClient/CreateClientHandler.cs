using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Clients.Commands.CreateClient;

public class CreateClientHandler(IClientRepository clientRepo, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateClientCommand, Guid>
{
    public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var existing = await clientRepo.GetByNitAsync(request.Nit, cancellationToken);
        if (existing != null)
            throw new DomainException($"Ya existe un cliente con NIT {request.Nit}.");

        var client = Client.Create(request.BusinessName, request.Nit, request.Type,
            request.ContactName, request.ContactPhone, request.ContactEmail);

        await clientRepo.AddAsync(client, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return client.Id;
    }
}
