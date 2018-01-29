using System.Collections.Generic;
using System.Linq;

namespace Robot.Expected.Extensions
{
    public static class IEnumerableExtensions
    {
        public static Expected<IEnumerable<T>> AsExpected<T>(this IEnumerable<T> enumerable)
        {
            return new Expected<IEnumerable<T>>(enumerable);
        }
    }

    public static class IQueryableExtensions
    {
        public static Expected<IQueryable<T>> AsExpected<T>(this IQueryable<T> enumerable)
        {
            return new Expected<IQueryable<T>>(enumerable);
        }
    }
}
