using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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