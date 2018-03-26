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
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StudioAdapters
{
    public class StudioMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media.Edge>
    {
        public StudioMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media.Edge>, IAniListError>> enumerable, RecyclerCardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Title?.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Node?.Format?.DisplayValue}{(item.Node?.IsAdult == true ? " (Hentai)" : "")}";
            holder.DetailSecondary.Visibility = item.IsMainStudio ? ViewStates.Visible : ViewStates.Gone;
            Context.LoadImage(holder.Image, item.Node?.CoverImage?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Text = "Main Studio";
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var mediaEdge = Items[mediaPos];

            MediaActivity.StartActivity(Context, mediaEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}