using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaStreamingEpisodesRecyclerAdapter : BaseRecyclerAdapter<Media.MediaStreamingEpisode>
    {
        public MediaStreamingEpisodesRecyclerAdapter(BaseAniDroidActivity context, List<Media.MediaStreamingEpisode> items) : base(context, items, RecyclerCardType.Vertical)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            Context.LoadImage(holder.Image, item.Thumbnail);
            holder.DetailPrimary.Text = item.Title;
            holder.DetailSecondary.Text = item.Site;

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        private void RowClick(object sender, EventArgs eventArgs)
        {
            var senderView = sender as View;
            var itemPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var item = Items[itemPos];

            var intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(item.Url));
            Context.StartActivity(intent);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailPrimary.SetMaxLines(2);
            item.DetailPrimary.Ellipsize = TextUtils.TruncateAt.End;
            item.Name.Visibility = ViewStates.Gone;
            return item;
        }
    }
}