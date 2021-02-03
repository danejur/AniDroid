using System;

namespace AniDroid.Utils.Comparers
{
    public class MediaListSortComparer : BaseAniDroidComparer<AniList.Models.MediaModels.MediaList>
    {
        private readonly MediaListSortType _sort;

        public MediaListSortComparer(MediaListSortType sort, SortDirection direction) : base(direction)
        {
            _sort = sort;
        }

        public override int CompareInternal(AniList.Models.MediaModels.MediaList x, AniList.Models.MediaModels.MediaList y)
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
                case MediaListSortType.DateStarted:
                    return SortDate(x, y, m => m.StartedAt?.GetFuzzyDate() ?? DateTime.MinValue);
                case MediaListSortType.DateCompleted:
                    return SortDate(x, y, m => m.CompletedAt?.GetFuzzyDate() ?? DateTime.MinValue);
                case MediaListSortType.NextEpisodeDate:
                    return SortDateTime(x, y, m => m.Media?.NextAiringEpisode?.GetAiringAtDateTime() ?? m.Media?.StartDate?.GetDate() ?? DateTime.MinValue);
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
            Duration = 9,
            DateStarted = 10,
            DateCompleted = 11,
            NextEpisodeDate = 12
        }
    }
}