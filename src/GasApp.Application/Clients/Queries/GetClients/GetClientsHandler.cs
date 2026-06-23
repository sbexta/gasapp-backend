using GasApp.Application.Common.Models;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Clients.Queries.GetClients;

public class GetClientsHandler(IClientRepository clientRepo)
    : IRequestHandler<GetClientsQuery, PagedResult<ClientDto>>
{
    public async Task<PagedResult<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await clientRepo.GetPagedAsync(
            request.Page, request.PageSize, request.Search, request.IsActive, cancellationToken);

        var dtos = items.Select(c => new ClientDto(
            c.Id, c.BusinessName, c.Nit, c.Type.ToString(),
            c.ContactName, c.ContactPhone, c.ContactEmail,
            c.IsActive, c.CreatedAt)).ToList();

        return new PagedResult<ClientDto>(dtos, total, request.Page, request.PageSize);
    }
}
