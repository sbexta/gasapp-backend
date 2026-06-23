using GasApp.Domain.Enums;
using MediatR;

namespace GasApp.Application.Clients.Commands.CreateClient;

public record CreateClientCommand(
    string BusinessName,
    string Nit,
    ClientType Type,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail
) : IRequest<Guid>;
