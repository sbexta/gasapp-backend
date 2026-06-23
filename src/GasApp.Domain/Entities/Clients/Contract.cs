using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Clients;

public class Contract : AuditableEntity
{
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public string ContractNumber { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public ContractStatus Status { get; private set; } = ContractStatus.Active;
    public string? Notes { get; private set; }

    private readonly List<Location> _locations = [];
    public IReadOnlyList<Location> Locations => _locations.AsReadOnly();

    private Contract() { }

    public static Contract Create(Guid clientId, string contractNumber, DateTime startDate, DateTime endDate, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new DomainException("El número de contrato no puede estar vacío.");

        if (endDate <= startDate)
            throw new DomainException("La fecha de fin debe ser posterior a la fecha de inicio.");

        return new Contract
        {
            ClientId = clientId,
            ContractNumber = contractNumber.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            Notes = notes?.Trim()
        };
    }

    public void Suspend()
    {
        if (Status != ContractStatus.Active)
            throw new DomainException("Solo se puede suspender un contrato activo.");
        Status = ContractStatus.Suspended;
        Touch();
    }

    public void Reactivate()
    {
        if (Status != ContractStatus.Suspended)
            throw new DomainException("Solo se puede reactivar un contrato suspendido.");
        Status = ContractStatus.Active;
        Touch();
    }

    public void Cancel()
    {
        if (Status == ContractStatus.Cancelled)
            throw new DomainException("El contrato ya está cancelado.");
        Status = ContractStatus.Cancelled;
        Touch();
    }

    public bool IsExpiredOn(DateTime date) => date > EndDate;
}
