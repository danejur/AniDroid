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
    public class MediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media>
    {
        public DetailType PrimaryDetailType { get; set; }
        public DetailType SecondaryDetailType { get; set; }
        public Action<Media> ClickAction { get; set; }
        public Action<Media> LongClickAction { get; set; }

        public User.UserMediaListOptions UserMediaListOptions { get; set; }

        public MediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, RecyclerCardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title.UserPreferred;
            holder.DetailPrimary.Text = GetDetail(PrimaryDetailType, item);
            holder.DetailSecondary.Text = GetDetail(SecondaryDetailType, item);
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

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            if (ClickAction != null)
            {
                ClickAction.Invoke(media);
            }
            else
            {
                MediaActivity.StartActivity(Context, media.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
            }
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var media = Items[mediaPos];

            if (LongClickAction != null)
            {
                LongClickAction.Invoke(media);
            }
            else
            {
                Context.DisplaySnackbarMessage(media.Title?.UserPreferred, Snackbar.LengthLong);
            }
        }

        protected string GetDetail(DetailType detailType, Media item)
        {
            switch (detailType)
            {
                case DetailType.Format:
                    return $"{item.Format?.DisplayValue}{(item.IsAdult ? " (Hentai)" : "")}";
                case DetailType.AverageRatingPopularity:
                    return $"{(item.AverageScore != 0 ? $"Avg Rating: {item.AverageScore}" : "No Rating Data")}\nPopularity: {item.Popularity}";
                case DetailType.UserScore:
                    return UserMediaListOptions != null ? item.MediaListEntry?.GetScoreString(UserMediaListOptions?.ScoreFormat) : "Score Unavailable";
                default:
                    return "";
            }
        }

        public enum DetailType
        {
            None,
            Format,
            AverageRatingPopularity,
            UserScore,
        }
    }
}