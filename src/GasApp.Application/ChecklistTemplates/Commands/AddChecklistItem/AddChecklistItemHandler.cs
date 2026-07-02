using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.AddChecklistItem;

public class AddChecklistItemHandler(
    IChecklistSectionRepository sectionRepo,
    IChecklistItemRepository itemRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddChecklistItemCommand, Guid>
{
    public async Task<Guid> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var section = await sectionRepo.GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException("Sección", request.SectionId);

        if (!Enum.TryParse<ChecklistItemType>(request.ItemType, out var itemType))
            throw new DomainException($"Tipo de ítem inválido: {request.ItemType}");

        var item = ChecklistItem.Create(section.Id, request.Question, itemType,
            request.Order, request.IsRequired, request.HelpText);

        await itemRepo.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
