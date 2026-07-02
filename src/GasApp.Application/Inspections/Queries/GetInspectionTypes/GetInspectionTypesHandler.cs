using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionTypes;

public class GetInspectionTypesHandler(IInspectionTypeRepository inspectionTypeRepository)
    : IRequestHandler<GetInspectionTypesQuery, IReadOnlyList<InspectionTypeResult>>
{
    public async Task<IReadOnlyList<InspectionTypeResult>> Handle(GetInspectionTypesQuery request, CancellationToken ct)
    {
        var types = await inspectionTypeRepository.GetAllActiveAsync(ct);
        return types
            .Select(t => new InspectionTypeResult(t.Id, t.Name, t.Description, t.RequiresCertificate))
            .ToList();
    }
}
