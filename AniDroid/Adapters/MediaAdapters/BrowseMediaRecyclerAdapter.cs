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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class BrowseMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media>
    {
        public Media.MediaSort SortType { get; set; } = Media.MediaSort.Id;

        public BrowseMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, CardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
            if (cardType == CardType.VerticalStaggered)
            {
                SetCardType(CardType.Vertical);
            }
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Format?.DisplayValue}{(item.IsAdult ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = GetSecondaryDetail(item, SortType);
            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.CoverImage.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
            holder.ContainerCard.LongClick -= RowLongClick;
            holder.ContainerCard.LongClick += RowLongClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.Name.SetSingleLine(false);
            item.Name.SetMaxLines(2);

            if (SortType == null || SortType == Media.MediaSort.Id)
            {
                item.DetailSecondary.Visibility = ViewStates.Gone;
            }

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            MediaActivity.StartActivity(Context, media.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            Context.DisplaySnackbarMessage(media.Title?.UserPreferred, Snackbar.LengthLong);
        }

        private static string GetSecondaryDetail(Media item, Media.MediaSort sortType)
        {
            var retString = "";

            if (Media.MediaSort.Popularity == sortType || Media.MediaSort.PopularityDesc == sortType)
            {
                retString = $"Popularity: {item.Popularity}";
            }
            else if (Media.MediaSort.Score == sortType || Media.MediaSort.ScoreDesc == sortType)
            {
                retString = $"Score: {item.AverageScore}%";
            }

            return retString;
        }
    }
}