using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.Search
{
    public class StudioSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Studio>
    {
        public StudioSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Studio>> enumerable) : base(context, enumerable, CardType.Horizontal)
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
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24dp);
            item.ButtonIcon.ImageTintList = FavoriteIconColor;

            item.Image.Visibility = item.DetailPrimary.Visibility = item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private static void RowClick(object sender, EventArgs e)
        {
            // TODO: start studio activity here
        }
    }
}