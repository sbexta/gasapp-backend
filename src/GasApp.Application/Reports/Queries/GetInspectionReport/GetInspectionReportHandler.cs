using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Reports.Queries.GetInspectionReport;

public class GetInspectionReportHandler(IReportRepository reportRepo)
    : IRequestHandler<GetInspectionReportQuery, InspectionReportResult>
{
    public async Task<InspectionReportResult> Handle(
        GetInspectionReportQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await reportRepo.GetInspectionReportAsync(
            request.From, request.To, request.Status, request.TechnicianId,
            request.Page, request.PageSize, cancellationToken);

        return new InspectionReportResult(items, total);
    }
}
