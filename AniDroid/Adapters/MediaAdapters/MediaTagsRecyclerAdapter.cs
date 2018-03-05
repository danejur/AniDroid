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
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaTagsRecyclerAdapter : BaseRecyclerAdapter<Media.MediaTag>
    {
        private readonly List<bool> _spoilerTags;

        public MediaTagsRecyclerAdapter(BaseAniDroidActivity context, List<Media.MediaTag> items) : base(context, items, CardType.Horizontal, 0)
        {
            _spoilerTags = items.Select(x => x.IsGeneralSpoiler || x.IsMediaSpoiler).ToList();
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];
            holder.Button.Click -= ButtonClick;

            if (_spoilerTags[position])
            {
                holder.Name.Text = "SPOILER";
                holder.DetailPrimary.Text = "Tap view button to show tag";
                holder.Button.Visibility = ViewStates.Visible;
                holder.Button.SetTag(Resource.Id.Object_Position, position);
                holder.Button.Click += ButtonClick;
            }
            else
            {
                holder.Name.Text = item.Name;
                holder.DetailPrimary.Text = item.Description;
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
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.ContainerCard.SetContentPadding(20, 20, 20, 20);
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_visibility_white_24px);
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            //var senderView = sender as View;
            //var studioPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            //var studioEdge = Items[studioPos];
        }
    }
}