using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media.Edge>
    {
        public StaffMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Media.Edge>> enumerable, CardType cardType) : base(context, enumerable, cardType, 3)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Title?.UserPreferred;
            holder.DetailPrimary.Text = $"{AniListEnum.GetDisplayValue<Media.MediaFormat>(item.Node?.Format)}{(item.Node?.IsAdult == true ? " (Hentai)" : "")}";
            holder.DetailSecondary.Text = item.StaffRole;
            Context.LoadImage(holder.Image, item.Node?.CoverImage?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);

            // TODO: implement start media activity click
        }
    }
}