using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
    string BusinessName,
    ClientType Type,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail
) : IRequest;
