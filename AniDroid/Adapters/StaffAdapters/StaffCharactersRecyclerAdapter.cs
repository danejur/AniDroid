using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Character;
using AniDroid.Base;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffCharactersRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Character.Edge>
    {
        public StaffCharactersRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Character.Edge>> enumerable, CardType cardType) : base(context, enumerable, cardType, 3)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Name?.GetFormattedName(true);
            holder.DetailPrimary.Text = AniListEnum.GetDisplayValue<Character.CharacterRole>(item.Role);
            Context.LoadImage(holder.Image, item.Node?.Image?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= CharacterClick;
            holder.ContainerCard.Click += CharacterClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private void CharacterClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var characterPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var characterEdge = Items[characterPos];

            CharacterActivity.StartActivity(Context, characterEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

    }
}