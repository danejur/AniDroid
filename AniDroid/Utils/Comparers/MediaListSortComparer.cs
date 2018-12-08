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
using AniDroid.AniList.Models;

namespace AniDroid.Utils.Comparers
{
    public class MediaListSortComparer : IComparer<Media.MediaList>
    {
        private readonly MediaListSortType _sort;
        private readonly MediaListSortDirection _direction;

        public MediaListSortComparer(MediaListSortType sort, MediaListSortDirection direction)
        {
            _sort = sort;
            _direction = direction;
        }

        public int Compare(Media.MediaList x, Media.MediaList y)
        {
            var firstMedia = x;
            var secondMedia = y;

            if (_direction == MediaListSortDirection.Descending)
            {
                secondMedia = x;
                firstMedia = y;
            }

            switch (_sort)
            {
                case MediaListSortType.NoSort:
                    return 0;
                case MediaListSortType.Title:
                    return SortString(firstMedia, secondMedia, m => m.Media.Title.UserPreferred);
                case MediaListSortType.Rating:
                    return SortNumber(firstMedia, secondMedia, m => m.Score);
                case MediaListSortType.Progress:
                    return SortNumber(firstMedia, secondMedia, m => m.Progress ?? 0);
                case MediaListSortType.Popularity:
                    return SortNumber(firstMedia, secondMedia, m => m.Media.Popularity);
                case MediaListSortType.AverageScore:
                    return SortNumber(firstMedia, secondMedia, m => m.Media.AverageScore);
                case MediaListSortType.DateReleased:
                    return SortDate(firstMedia, secondMedia, m => m.Media.StartDate?.GetFuzzyDate() ?? DateTime.MinValue);
                case MediaListSortType.DateAdded:
                    return SortDateTime(firstMedia, secondMedia, m => m.GetDateTimeOffset(m.CreatedAt).DateTime);
                case MediaListSortType.DateLastUpdated:
                    return SortDateTime(firstMedia, secondMedia, m => m.GetDateTimeOffset(m.UpdatedAt).DateTime);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int SortNumber(Media.MediaList x, Media.MediaList y, Func<Media.MediaList, double> numberSelector)
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

        private static int SortString(Media.MediaList x, Media.MediaList y, Func<Media.MediaList, string> stringSelector)
        {
            var xString = stringSelector(x);
            var yString = stringSelector(y);

            return string.Compare(xString, yString, StringComparison.InvariantCultureIgnoreCase);
        }

        private static int SortDate(Media.MediaList x, Media.MediaList y, Func<Media.MediaList, DateTime> dateSelector)
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

        private static int SortDateTime(Media.MediaList x, Media.MediaList y, Func<Media.MediaList, DateTime> dateTimeSelector)
        {
            var xDate = dateTimeSelector(x);
            var yDate = dateTimeSelector(y);

            return DateTime.Compare(xDate, yDate);
        }

        public enum MediaListSortType
        {
            NoSort = 0,
            Title = 1,
            Rating = 2,
            Progress = 3,
            AverageScore = 4,
            Popularity = 5,
            DateReleased = 6,
            DateAdded = 7,
            DateLastUpdated = 8
        }

        public enum MediaListSortDirection
        {
            Ascending = 1,
            Descending = 2
        }
    }
}