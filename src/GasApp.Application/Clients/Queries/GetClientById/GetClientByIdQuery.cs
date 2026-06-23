using MediatR;

namespace GasApp.Application.Clients.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDetailDto>;

public record ClientDetailDto(
    Guid Id,
    string BusinessName,
    string Nit,
    string Type,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<ContractSummaryDto> Contracts
);

public record ContractSummaryDto(
    Guid Id,
    string ContractNumber,
    DateTime StartDate,
    DateTime EndDate,
    string Status
);
