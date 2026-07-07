using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistItem;

public class UpdateChecklistItemHandler(
    IChecklistItemRepository itemRepo,
    IUnitOfWork uow) : IRequestHandler<UpdateChecklistItemCommand>
{
    public async Task Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepo.GetByIdAsync(request.ItemId, cancellationToken)
            ?? throw new NotFoundException("ÍtemChecklist", request.ItemId);

        if (!Enum.TryParse<ChecklistItemType>(request.ItemType, out var itemType))
            throw new DomainException($"Tipo de ítem inválido: {request.ItemType}");

        item.Update(request.Question, itemType, request.Order, request.IsRequired, request.HelpText);
        itemRepo.Update(item);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
