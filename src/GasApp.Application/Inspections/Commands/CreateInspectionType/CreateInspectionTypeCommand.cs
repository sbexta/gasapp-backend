using MediatR;

namespace GasApp.Application.Inspections.Commands.CreateInspectionType;

public record CreateInspectionTypeCommand(
    string Name,
    string? Description,
    bool RequiresCertificate
) : IRequest<Guid>;
