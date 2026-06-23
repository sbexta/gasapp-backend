using GasApp.Application.Common.Models;
using MediatR;

namespace GasApp.Application.Clients.Queries.GetClients;

public record GetClientsQuery(int Page = 1, int PageSize = 20, string? Search = null, bool? IsActive = null)
    : IRequest<PagedResult<ClientDto>>;

public record ClientDto(
    Guid Id,
    string BusinessName,
    string Nit,
    string Type,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive,
    DateTime CreatedAt
);
