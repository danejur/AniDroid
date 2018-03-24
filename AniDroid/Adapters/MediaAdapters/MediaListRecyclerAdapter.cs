using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListRecyclerAdapter : BaseRecyclerAdapter<Media.MediaList>
    {
        private readonly MediaListPresenter _presenter;
        private readonly User.UserMediaListOptions _mediaListOptions;

        public MediaListRecyclerAdapter(BaseAniDroidActivity context, List<Media.MediaList> items,
            User.UserMediaListOptions mediaListOptions, MediaListPresenter presenter, CardType cardType,
            int verticalCardColumns = 2) : base(context, items, cardType, verticalCardColumns)
        {
            _presenter = presenter;
            _mediaListOptions = mediaListOptions;
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Media.Title.UserPreferred;
            holder.DetailPrimary.Text = GetDetailOne(item);
            holder.DetailSecondary.Text = GetDetailTwo(item);
            holder.Button.Visibility = item.Status == Media.MediaListStatus.Current ? ViewStates.Visible : ViewStates.Gone;
            holder.ButtonIcon.SetTag(Resource.Id.Object_Position, position);
            Context.LoadImage(holder.Image, item.Media.CoverImage.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
            holder.ContainerCard.LongClick -= RowLongClick;
            holder.ContainerCard.LongClick += RowLongClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.ButtonIcon.SetImageResource(Resource.Drawable.svg_plus_circle_outline);
            item.ButtonIcon.Click -= ButtonClick;
            item.ButtonIcon.Click += ButtonClick;

            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaListPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var mediaList = Items[mediaListPos];

            MediaActivity.StartActivity(Context, mediaList.Media.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaListPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var mediaList = Items[mediaListPos];

            EditMediaListItemDialog.Create(Context, _presenter, mediaList.Media, mediaList, _mediaListOptions);
        }

        private void ButtonClick(object sender, EventArgs eventArgs)
        {
            var senderView = sender as View;
            var mediaListPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var mediaList = Items[mediaListPos];

            Context.DisplaySnackbarMessage($"Added episode to {mediaList.Media.Title.UserPreferred}");
        }

        private string GetDetailOne(Media.MediaList mediaList)
        {
            if (mediaList.Status == Media.MediaListStatus.Current)
            {
                if (mediaList.Media.Type == Media.MediaType.Anime)
                {
                    return $"Currently watched {mediaList.Progress ?? 0} out of {mediaList.Media.Episodes?.ToString() ?? "?"}";
                }
                if (mediaList.Media.Type == Media.MediaType.Manga)
                {
                    return $"Currently read {mediaList.Progress ?? 0} out of {mediaList.Media.Chapters?.ToString() ?? "?"}";
                }
            }

            return $"{mediaList.Media.Format?.DisplayValue}{(mediaList.Media.IsAdult ? " (Hentai)" : "")}";
        }

        private string GetDetailTwo(Media.MediaList mediaList)
        {
            return $"{mediaList.Media.Format?.DisplayValue}{(mediaList.Media.IsAdult ? " (Hentai)" : "")}";
        }
    }
}