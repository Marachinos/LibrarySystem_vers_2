using System.Linq.Expressions;

namespace LibrarySystem.Web.Extensions;

public static class QueryableExtensions
{
    public static IEnumerable<T> OrderByDynamic<T>(
        this IEnumerable<T> source,
        string propertyName,
        bool ascending)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, propertyName);
        var lambda = Expression.Lambda(property, param);

        var method = ascending ? "OrderBy" : "OrderByDescending";

        var result = typeof(Enumerable)
            .GetMethods()
            .Single(m => m.Name == method && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, new object[] { source, lambda.Compile() });

        return (IEnumerable<T>)result!;
    }
}