using System;
using Android.Support.V4.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;

namespace AniDroid.Adapters.SearchAdapters
{
    public class StaffSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Staff>
    {
        public StaffSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Staff>> enumerable, CardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = $"{item.Name.First} {item.Name.Last}";

            if (!string.IsNullOrWhiteSpace(item.Name?.Native))
            {
                holder.DetailPrimary.Visibility = ViewStates.Visible;
                holder.DetailPrimary.Text = item.Name.Native;
            }
            else
            {
                holder.DetailPrimary.Visibility = ViewStates.Gone;
            }

            holder.DetailSecondary.Text = item.Language?.DisplayValue ?? "(Language unknown)";
            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.Image?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var position = (int) senderView?.GetTag(Resource.Id.Object_Position);
            var staff = Items[position];

            StaffActivity.StartActivity(Context, staff.Id);
        }
    }
}