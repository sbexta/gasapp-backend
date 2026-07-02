using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplateDetail;

public class GetChecklistTemplateDetailHandler(IChecklistTemplateRepository repo)
    : IRequestHandler<GetChecklistTemplateDetailQuery, ChecklistTemplateDetailDto>
{
    public async Task<ChecklistTemplateDetailDto> Handle(
        GetChecklistTemplateDetailQuery request, CancellationToken cancellationToken)
    {
        var template = await repo.GetByIdWithSectionsAsync(request.TemplateId, cancellationToken)
            ?? throw new NotFoundException("Plantilla de checklist", request.TemplateId);

        return new ChecklistTemplateDetailDto(
            template.Id, template.Name, template.Description,
            template.InspectionTypeId, template.IsActive, template.Version,
            template.Sections.Select(s => new ChecklistSectionDetailDto(
                s.Id, s.Name, s.Order,
                s.Items.Select(i => new ChecklistItemDetailDto(
                    i.Id, i.Question, i.ItemType.ToString(),
                    i.IsRequired, i.Order, i.HelpText)).ToList()
            )).ToList());
    }
}
