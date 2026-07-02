using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.AddChecklistSection;

public class AddChecklistSectionHandler(
    IChecklistTemplateRepository templateRepo,
    IChecklistSectionRepository sectionRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddChecklistSectionCommand, Guid>
{
    public async Task<Guid> Handle(AddChecklistSectionCommand request, CancellationToken cancellationToken)
    {
        var template = await templateRepo.GetByIdAsync(request.TemplateId, cancellationToken)
            ?? throw new NotFoundException("Plantilla de checklist", request.TemplateId);

        var section = ChecklistSection.Create(template.Id, request.Name, request.Order);
        await sectionRepo.AddAsync(section, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return section.Id;
    }
}
