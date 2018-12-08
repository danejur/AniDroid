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

namespace AniDroid.Adapters.ViewModels
{
    public class MediaListViewModel : AniDroidAdapterViewModel<Media.MediaList>
    {
        public bool IsPriority { get; protected set; }
        public bool IsBehind { get; protected set; }
        public bool DisplayEpisodeProgressColor { get; protected set; }

        private readonly bool _displayTimeUntilAiringCountdown;

        public MediaListViewModel(Media.MediaList model, MediaListDetailType primaryMediaListDetailType, MediaListDetailType secondaryMediaListDetailType, bool displayTimeUntilAiringCountdown) : base(model)
        {
            _displayTimeUntilAiringCountdown = displayTimeUntilAiringCountdown;

            TitleText = model.Media?.Title?.UserPreferred;
            DetailPrimaryText = GetDetailString(model.Media?.Type, primaryMediaListDetailType);
            DetailSecondaryText = GetDetailString(model.Media?.Type, secondaryMediaListDetailType);
            ImageUri = model.Media?.CoverImage?.Large ?? model.Media?.CoverImage?.Medium;
            IsPriority = model.Priority > 0;



            if (Model.Media?.Type?.Equals(Media.MediaType.Anime) == true)
            {
                DisplayEpisodeProgressColor = Model.Status?.Equals(Media.MediaListStatus.Current) == true &&
                                              Model.Media.Status?.Equals(Media.MediaStatus.Releasing) == true &&
                                              Model.Media.NextAiringEpisode?.Episode > 1;

                if (DisplayEpisodeProgressColor)
                {
                    IsBehind = Model.Progress < Model.Media.NextAiringEpisode?.Episode;
                }
            }
        }

        public static MediaListViewModel CreateViewModel(Media.MediaList model)
        {
            var secondaryDetail = MediaListDetailType.Progress;

            if (model.Status?.Equals(Media.MediaListStatus.Planning) == true)
            {
                secondaryDetail = MediaListDetailType.EpisodeCountOrMovieLength;
            }
            else if (model.Status?.Equals(Media.MediaListStatus.Completed) == true)
            {
                secondaryDetail = MediaListDetailType.Rating;
            }

            return new MediaListViewModel(model, MediaListDetailType.FormatAndAiringInfo, secondaryDetail, true);
        }

        public enum MediaListDetailType
        {
            None,
            FormatAndAiringInfo,
            Progress,
            EpisodeCountOrMovieLength,
            Rating
        }

        private string GetDetailString(Media.MediaType mediaType, MediaListDetailType detailType)
        {
            string retString = null;

            if (mediaType?.Equals(Media.MediaType.Anime) == true)
            {
                retString = GetAnimeDetailString(detailType);
            }
            else if (mediaType?.Equals(Media.MediaType.Manga) == true)
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
                if (Model.Media?.Format?.Equals(Media.MediaFormat.Movie) == true)
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

            return retString;
        }
    }
}