using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Contracts.Commands.CreateContract;

public class CreateContractHandler(
    IContractRepository contractRepo,
    IClientRepository clientRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateContractCommand, Guid>
{
    public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.ClientId);

        if (!client.IsActive)
            throw new DomainException("No se puede crear un contrato para un cliente inactivo.");

        var existing = await contractRepo.GetByNumberAsync(request.ContractNumber, cancellationToken);
        if (existing != null)
            throw new DomainException($"Ya existe un contrato con número {request.ContractNumber}.");

        var contract = Contract.Create(request.ClientId, request.ContractNumber,
            request.StartDate, request.EndDate, request.Notes);

        await contractRepo.AddAsync(contract, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return contract.Id;
    }
}
