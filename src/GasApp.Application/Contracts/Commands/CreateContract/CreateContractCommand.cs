using MediatR;

namespace GasApp.Application.Contracts.Commands.CreateContract;

public record CreateContractCommand(
    Guid ClientId,
    string ContractNumber,
    DateTime StartDate,
    DateTime EndDate,
    string? Notes
) : IRequest<Guid>;
