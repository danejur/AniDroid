using System;
using Android.Support.V4.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Studio;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.SearchAdapters
{
    public class StudioSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Studio>
    {
        public StudioSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Studio>, IAniListError>> enumerable) : base(context, enumerable, CardType.Horizontal)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Name;
            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);
            item.ContainerCard.SetContentPadding(0, 20, 0, 20);
            item.Image.Visibility = item.DetailPrimary.Visibility = item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var position = (int)senderView?.GetTag(Resource.Id.Object_Position);
            var studio = Items[position];

            StudioActivity.StartActivity(Context, studio.Id);
        }
    }
}