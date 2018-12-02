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
    public class MediaViewModel : AniDroidAdapterViewModel<Media>
    {
        public Media.Edge ModelEdge { get; protected set; }

        private MediaViewModel(Media model, MediaDetailType primaryMediaDetailType, MediaDetailType secondaryMediaDetailType) : base(model)
        {
            TitleText = Model.Title?.UserPreferred;
            DetailPrimaryText = GetDetail(primaryMediaDetailType);
            DetailSecondaryText = GetDetail(secondaryMediaDetailType);
            ImageUri = model.CoverImage?.Large ?? model.CoverImage?.Medium;
        }

        public enum MediaDetailType
        {
            None,
            Format,
            FormatRating,
            Genres,
            UserScore,
        }

        public static MediaViewModel CreateMediaViewModel(Media model)
        {
            return new MediaViewModel(model, MediaDetailType.FormatRating, MediaDetailType.Genres);
        }

        private string GetDetail(MediaDetailType mediaDetailType)
        {
            var retString = "";

            if (mediaDetailType == MediaDetailType.Format)
            {
                retString = $"{Model.Format?.DisplayValue}{(Model.IsAdult ? " (Hentai)" : "")}";
            }
            else if (mediaDetailType == MediaDetailType.FormatRating)
            {
                retString = Model.Status?.EqualsAny(Media.MediaStatus.NotYetReleased, Media.MediaStatus.Cancelled) == true
                    ? Model.Format?.DisplayValue
                    : $"{Model.Format?.DisplayValue}  ({(Model.AverageScore != 0 ? $"{Model.AverageScore}%" : "No Rating Data")})";
            }
            else if (mediaDetailType == MediaDetailType.Genres)
            {
                retString = Model.Genres?.Any() == true ? string.Join(", ", Model.Genres) : "(No Genres)";
            }

            return retString;
        }
    }
}