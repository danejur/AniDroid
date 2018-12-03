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
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.User;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.UserAdapters
{
    public class UserFollowerRecyclerAdapter : LazyLoadingRecyclerViewAdapter<User>
    {
        public UserFollowerRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> enumerable, RecyclerCardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Name;
            holder.DetailPrimary.Visibility = item.DonatorTier > 0 ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.Avatar.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailPrimary.Text = "Donator";
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var userPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var user = Items[userPos];

            UserActivity.StartActivity(Context, user.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}