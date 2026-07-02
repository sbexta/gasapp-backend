using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.CreateChecklistTemplate;

public class CreateChecklistTemplateHandler(
    IChecklistTemplateRepository repo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateChecklistTemplateCommand, Guid>
{
    public async Task<Guid> Handle(CreateChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = ChecklistTemplate.Create(request.Name, request.Description, request.InspectionTypeId);
        await repo.AddAsync(template, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return template.Id;
    }
}
