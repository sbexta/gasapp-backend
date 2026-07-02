using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.SubmitChecklistResponse;

public class SubmitChecklistResponseHandler(
    IChecklistResponseRepository responseRepo,
    IInspectionRepository inspectionRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitChecklistResponseCommand>
{
    public async Task Handle(SubmitChecklistResponseCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        var existing = await responseRepo.GetByInspectionAndItemAsync(
            request.InspectionId, request.ChecklistItemId, cancellationToken);

        if (existing is not null)
        {
            existing.Update(request.TextValue, request.BoolValue, request.NumericValue,
                request.Complies, request.Notes);
            responseRepo.Update(existing);
        }
        else
        {
            var response = ChecklistResponse.Create(
                request.InspectionId, request.ChecklistItemId,
                request.TextValue, request.BoolValue, request.NumericValue,
                request.Complies, request.Notes);
            await responseRepo.AddAsync(response, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
