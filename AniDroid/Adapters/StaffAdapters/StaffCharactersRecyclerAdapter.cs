using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Character;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffCharactersRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Character.Edge>
    {
        public StaffCharactersRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Character.Edge>, IAniListError>> enumerable, CardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Name?.GetFormattedName(true);
            holder.DetailPrimary.Text = item.Role?.DisplayValue;
            Context.LoadImage(holder.Image, item.Node?.Image?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var characterPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var characterEdge = Items[characterPos];

            CharacterActivity.StartActivity(Context, characterEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}