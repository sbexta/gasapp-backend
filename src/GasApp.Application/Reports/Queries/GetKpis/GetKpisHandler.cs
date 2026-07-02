using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Reports.Queries.GetKpis;

public class GetKpisHandler(IReportRepository reportRepo)
    : IRequestHandler<GetKpisQuery, ReportKpis>
{
    public Task<ReportKpis> Handle(GetKpisQuery request, CancellationToken cancellationToken)
        => reportRepo.GetKpisAsync(request.From, request.To, cancellationToken);
}
