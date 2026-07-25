namespace Fullerene.Manager.Application.Util;

public sealed class PaginatedResult<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
}