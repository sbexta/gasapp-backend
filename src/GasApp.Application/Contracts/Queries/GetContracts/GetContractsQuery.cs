using MediatR;

namespace GasApp.Application.Contracts.Queries.GetContracts;

public record ContractResult(
    Guid Id,
    string ContractNumber,
    Guid ClientId,
    string ClientName,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    string? Notes
);

public record GetContractsQuery(Guid? ClientId = null) : IRequest<IReadOnlyList<ContractResult>>;
