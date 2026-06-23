using GasApp.Domain.Exceptions;
using GasApp.Domain.ValueObjects;

namespace GasApp.Domain.Entities.Clients;

public class Location : AuditableEntity
{
    public Guid ContractId { get; private set; }
    public Contract Contract { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string Municipality { get; private set; } = null!;
    public string Department { get; private set; } = null!;
    public GeoPoint? Coordinates { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Installation> _installations = [];
    public IReadOnlyList<Installation> Installations => _installations.AsReadOnly();

    private Location() { }

    public static Location Create(Guid contractId, string name, string address,
        string municipality, string department, GeoPoint? coordinates = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la sede no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("La dirección no puede estar vacía.");

        if (string.IsNullOrWhiteSpace(municipality))
            throw new DomainException("El municipio no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(department))
            throw new DomainException("El departamento no puede estar vacío.");

        return new Location
        {
            ContractId = contractId,
            Name = name.Trim(),
            Address = address.Trim(),
            Municipality = municipality.Trim(),
            Department = department.Trim(),
            Coordinates = coordinates
        };
    }

    public void Update(string name, string address, string municipality, string department, GeoPoint? coordinates)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la sede no puede estar vacío.");

        Name = name.Trim();
        Address = address.Trim();
        Municipality = municipality.Trim();
        Department = department.Trim();
        Coordinates = coordinates;
        Touch();
    }

    public void Activate() { IsActive = true; Touch(); }
    public void Deactivate() { IsActive = false; Touch(); }
}
