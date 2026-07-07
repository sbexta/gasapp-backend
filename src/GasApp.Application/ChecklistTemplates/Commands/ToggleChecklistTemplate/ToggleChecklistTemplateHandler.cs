using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.ToggleChecklistTemplate;

public class ToggleChecklistTemplateHandler(
    IChecklistTemplateRepository repo,
    IUnitOfWork uow) : IRequestHandler<ToggleChecklistTemplateCommand>
{
    public async Task Handle(ToggleChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await repo.GetByIdAsync(request.TemplateId, cancellationToken)
            ?? throw new NotFoundException("PlantillaChecklist", request.TemplateId);

        if (template.IsActive)
            template.Deactivate();
        else
            template.Activate();

        repo.Update(template);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
