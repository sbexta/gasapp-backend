using GasApp.Domain.Enums;
using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Reports.Queries.GetInspectionReport;

public record GetInspectionReportQuery(
    DateTime? From, DateTime? To,
    InspectionStatus? Status, Guid? TechnicianId,
    int Page = 1, int PageSize = 25)
    : IRequest<InspectionReportResult>;

public record InspectionReportResult(IReadOnlyList<ReportInspectionRow> Items, int Total);
