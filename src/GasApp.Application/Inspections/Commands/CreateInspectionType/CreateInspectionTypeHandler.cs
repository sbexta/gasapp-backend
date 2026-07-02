using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.CreateInspectionType;

public class CreateInspectionTypeHandler(
    IInspectionTypeRepository inspectionTypeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInspectionTypeCommand, Guid>
{
    public async Task<Guid> Handle(CreateInspectionTypeCommand request, CancellationToken ct)
    {
        var inspectionType = InspectionType.Create(request.Name, request.Description, request.RequiresCertificate);
        await inspectionTypeRepository.AddAsync(inspectionType, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return inspectionType.Id;
    }
}
