using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class DiscoverMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media>
    {
        private const int CardWidthDip = 150;

        private readonly int _cardWidth;

        public DiscoverMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Media>> enumerable) : base(context, enumerable, CardType.Vertical, 1)
        {
            _cardWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, CardWidthDip, context.Resources.DisplayMetrics);
            LoadingCardWidth = _cardWidth;
            LoadingCardHeight = ViewGroup.LayoutParams.MatchParent;
            SetHorizontalOrientation();
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Format?.DisplayValue}{(item.IsAdult ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = $"{(item.AverageScore != 0 ? $"Average Rating: {item.AverageScore}" : "No Rating Data")}      Popularity: {item.Popularity}";
            holder.Button.Visibility = item.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.CoverImage.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Container.LayoutParameters.Width = item.ContainerCard.LayoutParameters.Width = item.Image.LayoutParameters.Width = _cardWidth;
            item.Container.LayoutParameters.Height = item.ContainerCard.LayoutParameters.Height = item.Image.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;

            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            MediaActivity.StartActivity(Context, media.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}