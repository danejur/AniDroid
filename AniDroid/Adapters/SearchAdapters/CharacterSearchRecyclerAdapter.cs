using System;
using Android.Support.V4.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.SearchAdapters
{
    public class CharacterSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Character>
    {
        public CharacterSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Character>> enumerable, CardType cardType) : base(context, enumerable, cardType)
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

            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.Image?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24dp);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private static void RowClick(object sender, EventArgs e)
        {
            // TODO: start character activity here
        }
    }
}