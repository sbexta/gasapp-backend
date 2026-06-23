using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Clients.Commands.UpdateClient;

public class UpdateClientHandler(IClientRepository clientRepo, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);

        client.Update(request.BusinessName, request.Type,
            request.ContactName, request.ContactPhone, request.ContactEmail);

        clientRepo.Update(client);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
