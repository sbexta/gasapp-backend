using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Commands.CaptureLocation;

public class CaptureLocationHandler(
    IInspectionRepository inspectionRepo,
    IUnitOfWork uow) : IRequestHandler<CaptureLocationCommand>
{
    public async Task Handle(CaptureLocationCommand request, CancellationToken cancellationToken)
    {
        var inspection = await inspectionRepo.GetByIdAsync(request.InspectionId, cancellationToken)
            ?? throw new NotFoundException("Inspección", request.InspectionId);

        inspection.SetLocation(request.Latitude, request.Longitude);
        inspectionRepo.Update(inspection);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
