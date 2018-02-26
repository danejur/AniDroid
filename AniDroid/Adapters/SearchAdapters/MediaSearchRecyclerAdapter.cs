using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.SearchAdapters
{
    public class MediaSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media>
    {
        public MediaSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Media>> enumerable, CardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title.UserPreferred;
            holder.DetailPrimary.Text = $"{AniListEnum.GetDisplayValue<Media.MediaFormat>(item.Format)}{(item.IsAdult ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = $"{(item.AverageScore != 0 ? $"Average Rating: {item.AverageScore}": "No Rating Data")}      Popularity: {item.Popularity}";
            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.CoverImage.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24dp);
            item.ButtonIcon.ImageTintList = FavoriteIconColor;

            item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private static void RowClick(object sender, EventArgs e)
        {
            // TODO: start media activity here
        }
    }
}