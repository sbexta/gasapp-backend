using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence.Repositories;

public class ReportRepository(AppDbContext context) : IReportRepository
{
    public async Task<(IReadOnlyList<ReportInspectionRow> Items, int Total)> GetInspectionReportAsync(
        DateTime? from, DateTime? to, InspectionStatus? status, Guid? technicianId,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Inspections
            .Include(i => i.WorkOrder).ThenInclude(w => w.InspectionType)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(i => i.WorkOrder.ScheduledDate >= DateTime.SpecifyKind(from.Value, DateTimeKind.Utc));
        if (to.HasValue)
            query = query.Where(i => i.WorkOrder.ScheduledDate <= DateTime.SpecifyKind(to.Value, DateTimeKind.Utc));
        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);
        if (technicianId.HasValue)
            query = query.Where(i => i.TechnicianId == technicianId.Value);

        var total = await query.CountAsync(ct);

        var raw = await query
            .OrderByDescending(i => i.WorkOrder.ScheduledDate)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(i => new
            {
                i.Id,
                i.WorkOrder.OrderNumber,
                Status = i.Status.ToString(),
                i.WorkOrder.ScheduledDate,
                i.CompletedAt,
                i.TechnicianId,
                i.WorkOrder.LocationId
            })
            .ToListAsync(ct);

        var techIds = raw.Select(r => r.TechnicianId).Distinct().ToList();
        var techs = await context.Users
            .Where(u => techIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync(ct);
        var techMap = techs.ToDictionary(t => t.Id, t => t.FullName);

        var locIds = raw.Select(r => r.LocationId).Distinct().ToList();
        var locs = await context.Locations
            .Include(l => l.Contract).ThenInclude(c => c.Client)
            .Where(l => locIds.Contains(l.Id))
            .ToListAsync(ct);
        var locMap = locs.ToDictionary(l => l.Id);

        var inspIds = raw.Select(r => r.Id).ToList();
        var findingCounts = await context.Findings
            .Where(f => inspIds.Contains(f.InspectionId))
            .GroupBy(f => f.InspectionId)
            .Select(g => new { InspectionId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.InspectionId, g => g.Count, ct);

        var certSet = (await context.InspectionCertificates
            .Where(c => inspIds.Contains(c.InspectionId))
            .Select(c => c.InspectionId)
            .ToListAsync(ct)).ToHashSet();

        var rows = raw.Select(r =>
        {
            var loc = locMap.TryGetValue(r.LocationId, out var l) ? l : null;
            return new ReportInspectionRow(
                r.Id,
                r.OrderNumber,
                r.Status,
                r.ScheduledDate,
                r.CompletedAt,
                techMap.TryGetValue(r.TechnicianId, out var tn) ? tn : "—",
                loc?.Contract?.Client?.BusinessName ?? "—",
                loc?.Name ?? "—",
                findingCounts.TryGetValue(r.Id, out var fc) ? fc : 0,
                certSet.Contains(r.Id));
        }).ToList();

        return (rows, total);
    }

    public async Task<ReportKpis> GetKpisAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = context.Inspections.AsQueryable();

        if (from.HasValue)
            query = query.Where(i => i.WorkOrder.ScheduledDate >= DateTime.SpecifyKind(from.Value, DateTimeKind.Utc));
        if (to.HasValue)
            query = query.Where(i => i.WorkOrder.ScheduledDate <= DateTime.SpecifyKind(to.Value, DateTimeKind.Utc));

        var total = await query.CountAsync(ct);
        var completed = await query.CountAsync(i => i.Status == InspectionStatus.Completed, ct);
        var inProgress = await query.CountAsync(i => i.Status == InspectionStatus.InProgress, ct);
        var inReview = await query.CountAsync(i => i.Status == InspectionStatus.TechnicalReview, ct);
        var rejected = await query.CountAsync(i => i.Status == InspectionStatus.Rejected, ct);

        var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0;

        var durations = await query
            .Where(i => i.Status == InspectionStatus.Completed && i.StartedAt != null && i.CompletedAt != null)
            .Select(i => new { i.StartedAt, i.CompletedAt })
            .ToListAsync(ct);

        double? avgDays = durations.Count > 0
            ? Math.Round(durations.Average(d => (d.CompletedAt!.Value - d.StartedAt!.Value).TotalDays), 1)
            : null;

        return new ReportKpis(total, completed, inProgress, inReview, rejected, completionRate, avgDays);
    }
}
