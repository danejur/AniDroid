using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.SearchAdapters
{
    public class UserSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<User>
    {
        public UserSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<User>> enumerable, CardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Name;
            holder.DetailPrimary.Visibility = item.IsFollowing ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.Avatar?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            item.DetailPrimary.Text = "Following";
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private static void RowClick(object sender, EventArgs e)
        {
            // TODO: start user activity here
        }
    }
}