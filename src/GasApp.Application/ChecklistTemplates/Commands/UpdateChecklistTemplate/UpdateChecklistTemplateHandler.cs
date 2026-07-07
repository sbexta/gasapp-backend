using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistTemplate;

public class UpdateChecklistTemplateHandler(
    IChecklistTemplateRepository repo,
    IUnitOfWork uow) : IRequestHandler<UpdateChecklistTemplateCommand>
{
    public async Task Handle(UpdateChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await repo.GetByIdAsync(request.TemplateId, cancellationToken)
            ?? throw new NotFoundException("PlantillaChecklist", request.TemplateId);

        template.Update(request.Name, request.Description);
        repo.Update(template);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
