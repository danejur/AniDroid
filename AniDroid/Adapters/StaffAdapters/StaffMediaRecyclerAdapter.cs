using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Character;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media.Edge>
    {
        public StaffMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media.Edge>, IAniListError>> enumerable, CardType cardType) : base(context, enumerable, cardType, 3)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Title?.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Node?.Format?.DisplayValue}{(item.Node?.IsAdult == true ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = item.StaffRole;
            Context.LoadImage(holder.Image, item.Node?.CoverImage?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
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