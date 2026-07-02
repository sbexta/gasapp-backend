using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Inspections.Queries.GetInspectionHistory;

public class GetInspectionHistoryHandler(
    IInspectionStatusHistoryRepository historyRepo,
    IUserRepository userRepo)
    : IRequestHandler<GetInspectionHistoryQuery, IReadOnlyList<InspectionHistoryDto>>
{
    public async Task<IReadOnlyList<InspectionHistoryDto>> Handle(
        GetInspectionHistoryQuery request, CancellationToken cancellationToken)
    {
        var entries = await historyRepo.GetByInspectionIdAsync(request.InspectionId, cancellationToken);

        var userIds = entries.Where(e => e.ChangedById.HasValue)
            .Select(e => e.ChangedById!.Value).Distinct().ToList();

        var users = new Dictionary<Guid, string>();
        foreach (var uid in userIds)
        {
            var u = await userRepo.GetByIdAsync(uid, cancellationToken);
            if (u != null) users[uid] = u.FullName;
        }

        return entries.Select(e => new InspectionHistoryDto(
            e.Id,
            e.PreviousStatus?.ToString(),
            e.NewStatus.ToString(),
            e.ChangedAt,
            e.ChangedById,
            e.ChangedById.HasValue && users.TryGetValue(e.ChangedById.Value, out var name) ? name : null,
            e.Notes)).ToList();
    }
}
