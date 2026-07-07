using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistSection;

public class DeleteChecklistSectionHandler(
    IChecklistSectionRepository sectionRepo,
    IUnitOfWork uow) : IRequestHandler<DeleteChecklistSectionCommand>
{
    public async Task Handle(DeleteChecklistSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await sectionRepo.GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException("SecciónChecklist", request.SectionId);

        sectionRepo.Remove(section);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
