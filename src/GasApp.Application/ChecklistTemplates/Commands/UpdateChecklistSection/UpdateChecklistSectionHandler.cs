using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistSection;

public class UpdateChecklistSectionHandler(
    IChecklistSectionRepository sectionRepo,
    IUnitOfWork uow) : IRequestHandler<UpdateChecklistSectionCommand>
{
    public async Task Handle(UpdateChecklistSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await sectionRepo.GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException("SecciónChecklist", request.SectionId);

        section.Update(request.Name, request.Order);
        sectionRepo.Update(section);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
