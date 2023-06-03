using System.Linq.Expressions;

namespace ComUnity.Application.Common.Utils;

public static class IQueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool conditionMet, Expression<Func<T, bool>> selector)
    {
        return conditionMet ? source.Where(selector) : source;
    }

    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(this IQueryable<T> source, string direction, Expression<Func<T, TKey>> selector)
    {
        return direction == "asc" ? source.OrderBy(selector) : source.OrderByDescending(selector);
    }
}
