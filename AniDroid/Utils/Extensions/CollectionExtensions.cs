using System;
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

        public static IEnumerable<T> EveryNth<T>(this List<T> collection,
            int nStep)
        {
            for (var i = 0; i < collection.Count; i += nStep)
            {
                yield return collection[i];
            }
        }

        public static IEnumerable<T> EveryNthReverse<T>(this List<T> collection,
            int nStep)
        {
            for (var i = collection.Count - 1; i >= 0; i -= nStep)
            {
                yield return collection[i];
            }
        }

        // public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> selector)
        // {
        //     var seenKeys = new HashSet<TKey>();
        //     foreach (var element in collection)
        //     {
        //         if (seenKeys.Add(selector(element)))
        //         {
        //             yield return element;
        //         }
        //     }
        // }
    }
}