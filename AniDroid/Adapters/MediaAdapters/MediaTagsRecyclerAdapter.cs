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
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using AniDroid.Browse;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaTagsRecyclerAdapter : BaseRecyclerAdapter<MediaTag>
    {
        private readonly List<bool> _spoilerTags;
        private readonly MediaType _mediaType;

        public MediaTagsRecyclerAdapter(BaseAniDroidActivity context, List<MediaTag> items, MediaType mediaType) : base(context, items, RecyclerCardType.Horizontal)
        {
            _spoilerTags = items.Select(x => x.IsGeneralSpoiler || x.IsMediaSpoiler).ToList();
            _mediaType = mediaType;
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];
            holder.Button.Click -= ButtonClick;

            if (_spoilerTags[position])
            {
                holder.Name.Text = "SPOILER";
                holder.DetailPrimary.Text = "Tap view button to show tag";
                holder.DetailSecondary.Visibility = ViewStates.Gone;
                holder.Button.Visibility = ViewStates.Visible;
                holder.Button.SetTag(Resource.Id.Object_Position, position);
                holder.ContainerCard.Click -= RowClick;
                holder.Button.Click -= ButtonClick;
                holder.Button.Click += ButtonClick;
            }
            else
            {
                holder.Name.Text = item.Name;
                holder.DetailPrimary.Text = item.Description;
                holder.DetailSecondary.Visibility = ViewStates.Visible;
                holder.DetailSecondary.Text = $"{item.Rank}%";
                holder.Button.Visibility = ViewStates.Gone;

                holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
                holder.ContainerCard.Click -= RowClick;
                holder.ContainerCard.Click += RowClick;
            }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var tagPos = (int)senderView?.GetTag(Resource.Id.Object_Position);
            _spoilerTags[tagPos] = false;
            NotifyItemChanged(tagPos);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Image.Visibility = ViewStates.Gone;
            item.ContainerCard.SetContentPadding(20, 20, 20, 20);
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_visibility_white_24px);
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var itemPos = (int) senderView?.GetTag(Resource.Id.Object_Position);
            var item = Items[itemPos];

            BrowseActivity.StartActivity(Context, new BrowseMediaDto {Type = _mediaType, IncludedTags = new List<string> {item.Name}}, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}