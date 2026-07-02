using GasApp.Domain.Repositories;
using MediatR;

namespace GasApp.Application.Reports.Queries.GetKpis;

public record GetKpisQuery(DateTime? From, DateTime? To) : IRequest<ReportKpis>;
