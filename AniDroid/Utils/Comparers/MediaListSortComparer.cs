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
    public class MediaListSortComparer : BaseAniDroidComparer<Media.MediaList>
    {
        private readonly MediaListSortType _sort;

        public MediaListSortComparer(MediaListSortType sort, SortDirection direction) : base(direction)
        {
            _sort = sort;
        }

        public override int CompareInternal(Media.MediaList x, Media.MediaList y)
        {
            switch (_sort)
            {
                case MediaListSortType.NoSort:
                    return 0;
                case MediaListSortType.Title:
                    return SortString(x, y, m => m.Media.Title.UserPreferred);
                case MediaListSortType.Rating:
                    return SortNumber(x, y, m => m.Score);
                case MediaListSortType.Progress:
                    return SortNumber(x, y, m => m.Progress ?? 0);
                case MediaListSortType.Popularity:
                    return SortNumber(x, y, m => m.Media.Popularity);
                case MediaListSortType.AverageScore:
                    return SortNumber(x, y, m => m.Media.AverageScore);
                case MediaListSortType.DateReleased:
                    return SortDate(x, y, m => m.Media.StartDate?.GetFuzzyDate() ?? DateTime.MinValue);
                case MediaListSortType.DateAdded:
                    return SortDateTime(x, y, m => m.GetDateTimeOffset(m.CreatedAt).DateTime);
                case MediaListSortType.DateLastUpdated:
                    return SortDateTime(x, y, m => m.GetDateTimeOffset(m.UpdatedAt).DateTime);
                case MediaListSortType.Duration:
                    return SortNumber(x, y, m => m.Media.Duration);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            DateLastUpdated = 8,
            Duration = 9
        }
    }
}