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
        public MediaViewModel(Media model, DetailType primaryDetailType, DetailType secondaryDetailType) : base(model)
        {
            TitleText = Model.Title?.UserPreferred;
            DetailPrimaryText = GetDetail(primaryDetailType);
            DetailSecondaryText = GetDetail(secondaryDetailType);
            ImageUri = model.CoverImage?.Large ?? model.CoverImage?.Medium;
        }

        public enum DetailType
        {
            None,
            Format,
            FormatRating,
            Genres,
            UserScore,
        }

        private string GetDetail(DetailType detailType)
        {
            var retString = "";

            if (detailType == DetailType.Format)
            {
                retString = $"{Model.Format?.DisplayValue}{(Model.IsAdult ? " (Hentai)" : "")}";
            }
            else if (detailType == DetailType.FormatRating)
            {
                retString = Model.Status?.EqualsAny(Media.MediaStatus.NotYetReleased, Media.MediaStatus.Cancelled) == true
                    ? Model.Format?.DisplayValue
                    : $"{Model.Format?.DisplayValue}  ({(Model.AverageScore != 0 ? $"{Model.AverageScore}%" : "No Rating Data")})";
            }
            else if (detailType == DetailType.Genres)
            {
                retString = Model.Genres?.Any() == true ? string.Join(", ", Model.Genres) : "(No Genres)";
            }

            return retString;
        }
    }
}