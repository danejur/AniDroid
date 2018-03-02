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
using AniDroid.AniListObject.Studio;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaStudiosRecyclerAdapter : BaseRecyclerAdapter<Studio.Edge>
    {
        public MediaStudiosRecyclerAdapter(BaseAniDroidActivity context, List<Studio.Edge> items) : base(context, items, CardType.Horizontal, 0)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Name;
            holder.DetailPrimary.Visibility = item.IsMain ? ViewStates.Visible : ViewStates.Gone;

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Image.Visibility = ViewStates.Gone;
            item.DetailPrimary.Text = "Main Studio";
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.ContainerCard.SetContentPadding(0, 20, 0, 20);
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var studioPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var studioEdge = Items[studioPos];

            StudioActivity.StartActivity(Context, studioEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}