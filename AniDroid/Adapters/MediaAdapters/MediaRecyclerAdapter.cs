using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaRecyclerAdapter : LazyLoadingAniDroidRecyclerAdapter<MediaViewModel, Media>
    {
        public User.UserMediaListOptions UserMediaListOptions { get; set; }

        public MediaRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, RecyclerCardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override Action<AniDroidAdapterViewModel<Media>> ClickAction => viewModel =>
            MediaActivity.StartActivity(Context, viewModel.Model.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);

        public override Action<AniDroidAdapterViewModel<Media>> LongClickAction => viewModel =>
            Context.DisplaySnackbarMessage(viewModel.Model.Title?.UserPreferred, Snackbar.LengthLong);

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.TitleText;
            holder.DetailPrimary.Text = item.DetailPrimaryText;
            holder.DetailSecondary.Text = item.DetailSecondaryText;
            holder.Button.Visibility = item.Model.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.ImageUri);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.Name.SetSingleLine(false);
            item.Name.SetMaxLines(2);

            return item;
        }

        public static MediaRecyclerAdapter CreateBrowseMediaRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, RecyclerCardType cardType,
            Media.MediaSort sort)
        {
            return new MediaRecyclerAdapter(context, enumerable, cardType);
        }

        //private static MediaViewModel.MediaDetailType GetDetailType(Media.MediaSort sort)
        //{
        //    if (Media.MediaSort.Popularity == sort || Media.MediaSort.PopularityDesc == sort)
        //    {
        //        return MediaViewModel.MediaDetailType.AverageRatingPopularity
        //    }
        //    else if (Media.MediaSort.Score == sort || Media.MediaSort.ScoreDesc == sortsortType)
        //    {
        //        retString = $"Score: {item.AverageScore}%";
        //    }
        //}
    }
}