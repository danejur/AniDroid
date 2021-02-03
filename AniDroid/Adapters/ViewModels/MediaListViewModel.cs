using System;
using System.Text;
using Android.Graphics;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Enums.UserEnums;

namespace AniDroid.Adapters.ViewModels
{
    public class MediaListViewModel : AniDroidAdapterViewModel<AniList.Models.MediaModels.MediaList>
    {
        public bool IsPriority { get; protected set; }
        public MediaListWatchingStatus WatchingStatus { get; protected set; }
        public Color ImageColor { get; protected set; }

        private readonly bool _displayTimeUntilAiringCountdown;
        private readonly ScoreFormat _scoreFormat;
        private readonly MediaListRecyclerAdapter.MediaListProgressDisplayType _progressDisplayType;

        public MediaListViewModel(AniList.Models.MediaModels.MediaList model, MediaListDetailType primaryMediaListDetailType,
            MediaListDetailType secondaryMediaListDetailType, bool displayTimeUntilAiringCountdown,
            MediaListRecyclerAdapter.MediaListProgressDisplayType progressDisplayType,
            ScoreFormat scoreFormat) : base(model)
        {
            _displayTimeUntilAiringCountdown = displayTimeUntilAiringCountdown;
            _scoreFormat = scoreFormat;
            _progressDisplayType = progressDisplayType;

            TitleText = model.Media?.Title?.UserPreferred;
            DetailPrimaryText = GetDetailString(model.Media?.Type, primaryMediaListDetailType);
            DetailSecondaryText = GetDetailString(model.Media?.Type, secondaryMediaListDetailType);
            ImageUri = model.Media?.CoverImage?.Large ?? model.Media?.CoverImage?.Medium;
            IsPriority = model.Priority > 0;
            ButtonIcon = GetEpisodeAddIcon();
            ImageColor = Color.ParseColor(model.Media?.CoverImage?.Color ?? "#00000000");
            WatchingStatus = GetWatchingStatus();
        }

        public static MediaListViewModel CreateViewModel(AniList.Models.MediaModels.MediaList model, ScoreFormat scoreFormat, bool displayTimeUntilAiringCountdown,
            MediaListRecyclerAdapter.MediaListProgressDisplayType progressDisplayType, bool readOnly, bool showEpisodeAddButtonForRepeatingMedia)
        {
            var secondaryDetail = MediaListDetailType.Progress;

            if (model.Status?.Equals(MediaListStatus.Planning) == true)
            {
                secondaryDetail = MediaListDetailType.EpisodeCountOrMovieLength;
            }
            else if (model.Status?.Equals(MediaListStatus.Completed) == true)
            {
                secondaryDetail = MediaListDetailType.Rating;
            }

            return new MediaListViewModel(model, MediaListDetailType.FormatAndAiringInfo, secondaryDetail, displayTimeUntilAiringCountdown,
                progressDisplayType, scoreFormat)
            {
                IsButtonVisible = !readOnly && (model.Status?.Equals(MediaListStatus.Current) == true || showEpisodeAddButtonForRepeatingMedia && model.Status?.Equals(MediaListStatus.Repeating) == true)
            };
        }

        public static MediaListViewModel CreateUserMediaListViewModel(AniList.Models.MediaModels.MediaList model)
        {
            var retModel = new MediaListViewModel(model, MediaListDetailType.None, MediaListDetailType.None, false,
                MediaListRecyclerAdapter.MediaListProgressDisplayType.Never, null)
            {
                TitleText = model.User?.Name,
                DetailPrimaryText = model.Status?.DisplayValue,
                ImageUri = model.User?.Avatar?.Large ?? model.User?.Avatar?.Medium
            };

            retModel.DetailSecondaryText = model.Media?.Status?.Equals(MediaStatus.Releasing) == true &&
                                           model.Status?.EqualsAny(MediaListStatus.Current,
                                               MediaListStatus.Paused, MediaListStatus.Dropped,
                                               MediaListStatus.Planning) == true
                ? retModel.GetDetailString(model.Media.Type, MediaListDetailType.Progress)
                : model.GetScoreString(model.User?.MediaListOptions?.ScoreFormat);

            return retModel;
        }

        public enum MediaListDetailType
        {
            None,
            FormatAndAiringInfo,
            Progress,
            EpisodeCountOrMovieLength,
            Rating
        }

        public enum MediaListWatchingStatus
        {
            None,
            UpToDate,
            SlightlyBehind,
            Behind,
            VeryBehind,
        }

        private int GetEpisodeAddIcon()
        {
            int? totalCount = 0;

            if (Model.Media?.Type?.Equals(MediaType.Anime) == true)
            {
                totalCount = Model.Media.Episodes;
            }
            else if (Model.Media?.Type?.Equals(MediaType.Manga) == true)
            {
                totalCount = Model.Media.Chapters;
            }

            return totalCount.HasValue && totalCount <= Model.Progress + 1
                ? Resource.Drawable.svg_check_circle_outline
                : Resource.Drawable.svg_plus_circle_outline;
        }

        private string GetDetailString(MediaType mediaType, MediaListDetailType detailType)
        {
            string retString = null;

            if (mediaType?.Equals(MediaType.Anime) == true)
            {
                retString = GetAnimeDetailString(detailType);
            }
            else if (mediaType?.Equals(MediaType.Manga) == true)
            {
                retString = GetMangaDetailString(detailType);
            }

            return retString;
        }

        private string GetAnimeDetailString(MediaListDetailType detailType)
        {
            string retString = null;

            if (detailType == MediaListDetailType.FormatAndAiringInfo)
            {
                var stringBuilder =
                    new StringBuilder($"{Model.Media.Format?.DisplayValue}{(Model.Media.IsAdult ? " (Hentai)" : "")}");

                if (Model.Media?.NextAiringEpisode != null)
                {
                    stringBuilder.Append($"  (Episode {Model.Media.NextAiringEpisode.Episode}:  ");
                    stringBuilder.Append(!_displayTimeUntilAiringCountdown
                        ? Model.Media.NextAiringEpisode.GetAiringAtDateTime().ToShortDateString()
                        : Model.Media.NextAiringEpisode.GetExactDurationString(Model.Media.NextAiringEpisode
                            .TimeUntilAiring));
                    stringBuilder.Append(")");
                }

                retString = stringBuilder.ToString();
            }
            else if (detailType == MediaListDetailType.EpisodeCountOrMovieLength)
            {
                if (Model.Media?.Format?.Equals(MediaFormat.Movie) == true)
                {
                    retString = Model.Media.Duration.HasValue
                        ? Model.Media.GetExactDurationString(Model.Media.Duration.Value * 60)
                        : "Duration Unknown";
                }
                else
                {
                    // wew
                    retString =
                        $"{((Model.Media?.Episodes ?? 0) > 0 ? Model.Media?.Episodes?.ToString() : "?")} episode{(Model.Media?.Episodes > 1 ? "s" : "")}  /  {(Model.Media?.Duration.HasValue == true ? $"{Model.Media.GetExactDurationString(Model.Media.Duration.Value * 60)}" : "")}";
                }
            }
            else if (detailType == MediaListDetailType.Progress)
            {
                if (Model.Progress.HasValue && Model.Progress > 0 &&
                    Model.Progress == Model.Media?.Episodes)
                {
                    retString = "Status needs to be marked as Completed";
                }
                else
                {
                    retString =
                        $"Watched {Model.Progress ?? 0} out of {((Model.Media?.Episodes ?? 0) > 0 ? Model.Media?.Episodes?.ToString() : "?")}";
                }
            }
            else if (detailType == MediaListDetailType.Rating)
            {
                retString = Model.GetScoreString(_scoreFormat);
            }

            return retString;
        }

        private string GetMangaDetailString(MediaListDetailType detailType)
        {
            string retString = null;

            if (detailType == MediaListDetailType.FormatAndAiringInfo)
            {
                retString = $"{Model.Media.Format?.DisplayValue}{(Model.Media.IsAdult ? " (Hentai)" : "")}";
            }
            else if (detailType == MediaListDetailType.EpisodeCountOrMovieLength)
            {
                var stringBuilder =
                    new StringBuilder(
                        $"{((Model.Media?.Chapters ?? 0) > 0 ? Model.Media?.Chapters?.ToString() : "?")} chapter{(Model.Media?.Chapters > 1 ? "s" : "")}");

                if (Model.Media?.Volumes > 0)
                {
                    stringBuilder.Append($"  /  {Model.Media.Volumes} volume{(Model.Media.Volumes > 1 ? "s" : "")}");
                }

                retString = stringBuilder.ToString();
            }
            else if (detailType == MediaListDetailType.Progress)
            {
                if (Model.Progress.HasValue && Model.Progress > 0 &&
                    Model.Progress == Model.Media?.Chapters)
                {
                    retString = "Status needs to be marked as Completed";
                }
                else
                {
                    retString =
                        $"Read {Model.Progress ?? 0} out of {((Model.Media?.Chapters ?? 0) > 0 ? Model.Media?.Chapters?.ToString() : "?")}";
                }
            }
            else if (detailType == MediaListDetailType.Rating)
            {
                retString = Model.GetScoreString(_scoreFormat);
            }

            return retString;
        }

        private MediaListWatchingStatus GetWatchingStatus()
        {
            var retStatus = MediaListWatchingStatus.None;

            // only display colors if the show is properly aligned with what the user wants to see
            var shouldDisplayStatus = _progressDisplayType == MediaListRecyclerAdapter.MediaListProgressDisplayType.Always;
            shouldDisplayStatus |= _progressDisplayType ==
                                  MediaListRecyclerAdapter.MediaListProgressDisplayType.Releasing &&
                                  Model.Media?.Status?.Equals(MediaStatus.Releasing) == true;
            shouldDisplayStatus |= _progressDisplayType ==
                                   MediaListRecyclerAdapter.MediaListProgressDisplayType.ReleasingExtended &&
                                   (Model.Media?.Status?.Equals(MediaStatus.Releasing) == true ||
                                   Model.Media?.Status?.Equals(MediaStatus.Finished) == true &&
                                   Model.Media?.EndDate?.GetDate() > DateTime.Now.AddDays(-7));

            if (!shouldDisplayStatus || Model.Media?.Type?.Equals(MediaType.Anime) != true ||
                Model.Status?.Equals(MediaListStatus.Current) != true)
            {
                return MediaListWatchingStatus.None;
            }

            var totalAiredEpisodes = Model.Media.NextAiringEpisode?.Episode - 1 ?? Model.Media.Episodes;

            if (Model.Progress >= totalAiredEpisodes)
            {
                retStatus = MediaListWatchingStatus.UpToDate;
            }
            else if (Model.Progress >= totalAiredEpisodes - 1)
            {
                retStatus = MediaListWatchingStatus.SlightlyBehind;
            }
            else if (Model.Progress >= totalAiredEpisodes - 3)
            {
                retStatus = MediaListWatchingStatus.Behind;
            }
            else if (Model.Progress < totalAiredEpisodes - 3)
            {
                retStatus = MediaListWatchingStatus.VeryBehind;
            }

            return retStatus;
        }
    }
}