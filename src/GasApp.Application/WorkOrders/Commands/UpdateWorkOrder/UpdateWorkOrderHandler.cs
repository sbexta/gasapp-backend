using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderHandler(IWorkOrderRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateWorkOrderCommand>
{
    public async Task Handle(UpdateWorkOrderCommand request, CancellationToken ct)
    {
        var order = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("WorkOrder", request.Id);

        order.UpdateDetails(request.ScheduledDate, request.Notes);
        repo.Update(order);
        await uow.SaveChangesAsync(ct);
    }
}
