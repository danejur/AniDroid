using System;
using Android.Support.Design.Widget;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.SearchAdapters
{
    public class MediaSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media>
    {
        public MediaSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, RecyclerCardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Format?.DisplayValue}{(item.IsAdult ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = $"{(item.AverageScore != 0 ? $"Average Rating: {item.AverageScore}": "No Rating Data")}      Popularity: {item.Popularity}";
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

            item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            MediaActivity.StartActivity(Context, media.Id);
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            Context.DisplaySnackbarMessage(media.Title?.UserPreferred, Snackbar.LengthLong);
        }
    }
}