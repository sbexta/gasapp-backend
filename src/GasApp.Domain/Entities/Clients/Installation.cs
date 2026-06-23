using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Clients;

public class Installation : AuditableEntity
{
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public InstallationType Type { get; private set; }
    public InstallationStatus Status { get; private set; } = InstallationStatus.Active;
    public string? SerialNumber { get; private set; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public int? InstallationYear { get; private set; }
    public string? Notes { get; private set; }

    private Installation() { }

    public static Installation Create(Guid locationId, string name, InstallationType type,
        string? serialNumber = null, string? brand = null, string? model = null,
        int? installationYear = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la instalación no puede estar vacío.");

        return new Installation
        {
            LocationId = locationId,
            Name = name.Trim(),
            Type = type,
            SerialNumber = serialNumber?.Trim(),
            Brand = brand?.Trim(),
            Model = model?.Trim(),
            InstallationYear = installationYear,
            Notes = notes?.Trim()
        };
    }

    public void UpdateStatus(InstallationStatus newStatus)
    {
        Status = newStatus;
        Touch();
    }

    public void Update(string name, InstallationType type, string? serialNumber,
        string? brand, string? model, int? installationYear, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la instalación no puede estar vacío.");

        Name = name.Trim();
        Type = type;
        SerialNumber = serialNumber?.Trim();
        Brand = brand?.Trim();
        Model = model?.Trim();
        InstallationYear = installationYear;
        Notes = notes?.Trim();
        Touch();
    }
}
