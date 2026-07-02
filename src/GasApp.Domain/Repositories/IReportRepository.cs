using GasApp.Domain.Enums;

namespace GasApp.Domain.Repositories;

public interface IReportRepository
{
    Task<(IReadOnlyList<ReportInspectionRow> Items, int Total)> GetInspectionReportAsync(
        DateTime? from, DateTime? to, InspectionStatus? status, Guid? technicianId,
        int page, int pageSize, CancellationToken ct = default);

    Task<ReportKpis> GetKpisAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
}

public record ReportInspectionRow(
    Guid InspectionId,
    string OrderNumber,
    string Status,
    DateTime ScheduledDate,
    DateTime? CompletedAt,
    string TechnicianName,
    string ClientName,
    string LocationName,
    int FindingsCount,
    bool HasCertificate);

public record ReportKpis(
    int Total,
    int Completed,
    int InProgress,
    int InReview,
    int Rejected,
    double CompletionRate,
    double? AvgCompletionDays);
