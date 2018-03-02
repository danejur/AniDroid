using System;
using System.Linq;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Character;
using AniDroid.Base;
using AniDroid.Dialogs;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaCharactersRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Character.Edge>
    {
        public MediaCharactersRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Character.Edge>> enumerable, CardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Name?.GetFormattedName(true);
            holder.DetailPrimary.Text = AniListEnum.GetDisplayValue<Character.CharacterRole>(item.Role);
            Context.LoadImage(holder.Image, item.Node?.Image?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;

            if (item.VoiceActors?.Any() == true)
            {
                holder.Button.Visibility = ViewStates.Visible;
                holder.Button.SetTag(Resource.Id.Object_Position, position);
                holder.Button.Click -= ViewVoiceActorsClick;
                holder.Button.Click += ViewVoiceActorsClick;
            }
            else
            {
                holder.Button.Visibility = ViewStates.Gone;
            }
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_record_voice_over_white_24dp);
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var characterPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var characterEdge = Items[characterPos];

            CharacterActivity.StartActivity(Context, characterEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

        private void ViewVoiceActorsClick(object sender, EventArgs e)
        {
            var view = sender as View;
            var position = (int)view.GetTag(Resource.Id.Object_Position);
            var item = Items[position];
            StaffListDialog.Create(Context, item.VoiceActors);
        }
    }
}