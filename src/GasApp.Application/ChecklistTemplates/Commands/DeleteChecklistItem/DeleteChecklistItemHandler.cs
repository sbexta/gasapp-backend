using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistItem;

public class DeleteChecklistItemHandler(
    IChecklistItemRepository itemRepo,
    IUnitOfWork uow) : IRequestHandler<DeleteChecklistItemCommand>
{
    public async Task Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepo.GetByIdAsync(request.ItemId, cancellationToken)
            ?? throw new NotFoundException("ÍtemChecklist", request.ItemId);

        itemRepo.Remove(item);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
