using GasApp.Domain.Exceptions;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.WorkOrders.Commands.CancelWorkOrder;

public class CancelWorkOrderHandler(IWorkOrderRepository repo, IUnitOfWork uow)
    : IRequestHandler<CancelWorkOrderCommand>
{
    public async Task Handle(CancelWorkOrderCommand request, CancellationToken ct)
    {
        var order = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("WorkOrder", request.Id);

        order.Cancel(request.Reason);
        repo.Update(order);
        await uow.SaveChangesAsync(ct);
    }
}
