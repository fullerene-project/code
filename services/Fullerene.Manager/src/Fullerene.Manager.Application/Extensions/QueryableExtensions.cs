using System.Linq.Expressions;

namespace Fullerene.Manager.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> queryable,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? queryable.Where(predicate) : queryable;
    }

    public static IQueryable<T> Page<T>(
        this IQueryable<T> enumerable, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        return enumerable.Skip((page - 1) * pageSize).Take(pageSize);
    }
}