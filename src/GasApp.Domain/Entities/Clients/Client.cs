using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Clients;

public class Client : AuditableEntity
{
    public string BusinessName { get; private set; } = null!;
    public string Nit { get; private set; } = null!;
    public ClientType Type { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Contract> _contracts = [];
    public IReadOnlyList<Contract> Contracts => _contracts.AsReadOnly();

    private Client() { }

    public static Client Create(string businessName, string nit, ClientType type,
        string? contactName = null, string? contactPhone = null, string? contactEmail = null)
    {
        if (string.IsNullOrWhiteSpace(businessName))
            throw new DomainException("La razón social no puede estar vacía.");

        if (string.IsNullOrWhiteSpace(nit))
            throw new DomainException("El NIT no puede estar vacío.");

        return new Client
        {
            BusinessName = businessName.Trim(),
            Nit = nit.Trim(),
            Type = type,
            ContactName = contactName?.Trim(),
            ContactPhone = contactPhone?.Trim(),
            ContactEmail = contactEmail?.Trim()
        };
    }

    public void Update(string businessName, ClientType type,
        string? contactName, string? contactPhone, string? contactEmail)
    {
        if (string.IsNullOrWhiteSpace(businessName))
            throw new DomainException("La razón social no puede estar vacía.");

        BusinessName = businessName.Trim();
        Type = type;
        ContactName = contactName?.Trim();
        ContactPhone = contactPhone?.Trim();
        ContactEmail = contactEmail?.Trim();
        Touch();
    }

    public void Activate() { IsActive = true; Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
}
