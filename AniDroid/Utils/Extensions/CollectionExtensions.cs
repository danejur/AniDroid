using System.Collections.Generic;
using System.Linq;

namespace AniDroid.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> collection, params T[] values)
        {
            return values.Any(collection.Contains);
        }

        public static bool ContainsAny<T>(this IEnumerable<T> collection, IEnumerable<T> values)
        {
            return values.Any(collection.Contains);
        }
    }
}