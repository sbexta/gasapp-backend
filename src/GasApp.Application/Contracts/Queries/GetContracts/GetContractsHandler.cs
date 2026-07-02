using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Contracts.Queries.GetContracts;

public class GetContractsHandler(IContractRepository contractRepository)
    : IRequestHandler<GetContractsQuery, IReadOnlyList<ContractResult>>
{
    public async Task<IReadOnlyList<ContractResult>> Handle(GetContractsQuery request, CancellationToken ct)
    {
        var contracts = await contractRepository.GetAllAsync(request.ClientId, ct);
        return contracts
            .Select(c => new ContractResult(
                c.Id,
                c.ContractNumber,
                c.ClientId,
                c.Client?.BusinessName ?? string.Empty,
                c.StartDate,
                c.EndDate,
                c.Status.ToString(),
                c.Notes))
            .ToList();
    }
}
