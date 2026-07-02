using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.AddFinding;

public class AddFindingHandler(
    IFindingRepository findingRepo,
    IInspectionRepository inspectionRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddFindingCommand, Guid>
{
    public async Task<Guid> Handle(AddFindingCommand request, CancellationToken cancellationToken)
    {
        _ = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var finding = Finding.Create(
            request.InspectionId, request.Description, request.Severity,
            request.RequiresCorrection, request.ChecklistItemId, request.CorrectiveAction);

        await findingRepo.AddAsync(finding, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return finding.Id;
    }
}
