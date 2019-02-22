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

namespace AniDroid.Utils.Comparers
{
    public abstract class BaseAniDroidComparer<T> : IComparer<T>
    {
        private readonly SortDirection _direction;

        protected BaseAniDroidComparer(SortDirection direction)
        {
            _direction = direction;
        }

        public abstract int CompareInternal(T x, T y);

        public int Compare(T x, T y)
        {
            var firstItem = x;
            var secondItem = y;

            if (_direction == SortDirection.Descending)
            {
                secondItem = x;
                firstItem = y;
            }

            return CompareInternal(firstItem, secondItem);
        }

        protected static int SortNumber(T x, T y, Func<T, double?> numberSelector)
        {
            var xNum = numberSelector(x);
            var yNum = numberSelector(y);

            if (xNum > yNum)
            {
                return 1;
            }

            if (yNum > xNum)
            {
                return -1;
            }

            return 0;
        }

        protected static int SortString(T x, T y, Func<T, string> stringSelector)
        {
            var xString = stringSelector(x);
            var yString = stringSelector(y);

            return string.Compare(xString, yString, StringComparison.InvariantCultureIgnoreCase);
        }

        protected static int SortDate(T x, T y, Func<T, DateTime> dateSelector)
        {
            var xDate = dateSelector(x);
            var yDate = dateSelector(y);

            if (xDate.Date > yDate.Date)
            {
                return 1;
            }

            if (yDate.Date > xDate.Date)
            {
                return -1;
            }

            return 0;
        }

        protected static int SortDateTime(T x, T y, Func<T, DateTime> dateTimeSelector)
        {
            var xDate = dateTimeSelector(x);
            var yDate = dateTimeSelector(y);

            return DateTime.Compare(xDate, yDate);
        }

        public enum SortDirection
        {
            Ascending = 1,
            Descending = 2
        }
    }
}