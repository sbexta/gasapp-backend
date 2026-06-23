using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Installations.Commands.CreateInstallation;

public record CreateInstallationCommand(
    Guid LocationId,
    string Name,
    InstallationType Type,
    string? SerialNumber,
    string? Brand,
    string? Model,
    int? InstallationYear,
    string? Notes
) : IRequest<Guid>;
