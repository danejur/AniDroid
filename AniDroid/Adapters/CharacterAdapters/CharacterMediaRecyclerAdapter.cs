using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Dialogs;
using OneOf;

namespace AniDroid.Adapters.CharacterAdapters
{
    public class CharacterMediaRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Media.Edge>
    {
        public CharacterMediaRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Media.Edge>, IAniListError>> enumerable, RecyclerCardType cardType) : base(context, enumerable, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Title?.UserPreferred;
            holder.DetailPrimary.Text = $"{item.Node?.Format?.DisplayValue}{(item.Node?.IsAdult == true ? " (Hentai)" : "")}";
            Context.LoadImage(holder.Image, item.Node?.CoverImage?.Large);

            if (Media.MediaType.Anime.Equals(item.Node?.Type) && item.VoiceActors?.Any() == true)
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

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_record_voice_over_white_24px);
            return item;
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