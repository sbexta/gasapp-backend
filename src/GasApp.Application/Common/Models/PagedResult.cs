namespace GasApp.Application.Common.Models;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Page,
    int PageSize)
{
    public int Pages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
}
