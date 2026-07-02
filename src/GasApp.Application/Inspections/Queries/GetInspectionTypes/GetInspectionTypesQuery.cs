using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionTypes;

public record InspectionTypeResult(Guid Id, string Name, string? Description, bool RequiresCertificate);

public record GetInspectionTypesQuery : IRequest<IReadOnlyList<InspectionTypeResult>>;
