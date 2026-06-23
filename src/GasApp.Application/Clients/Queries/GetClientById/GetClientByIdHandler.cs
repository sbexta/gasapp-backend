using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Clients.Queries.GetClientById;

public class GetClientByIdHandler(IClientRepository clientRepo, IContractRepository contractRepo)
    : IRequestHandler<GetClientByIdQuery, ClientDetailDto>
{
    public async Task<ClientDetailDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await clientRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);

        var contracts = await contractRepo.GetByClientIdAsync(client.Id, cancellationToken);

        var contractDtos = contracts.Select(c => new ContractSummaryDto(
            c.Id, c.ContractNumber, c.StartDate, c.EndDate, c.Status.ToString())).ToList();

        return new ClientDetailDto(
            client.Id, client.BusinessName, client.Nit, client.Type.ToString(),
            client.ContactName, client.ContactPhone, client.ContactEmail,
            client.IsActive, client.CreatedAt, contractDtos);
    }
}
