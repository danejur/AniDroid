using System;
using System.Linq;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.SearchResults;

namespace AniDroid.Adapters.SearchAdapters
{
    public class CharacterSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Character>
    {
        private readonly SearchResultsPresenter _presenter;

        public CharacterSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Character>> enumerable, CardType cardType, SearchResultsPresenter presenter) : base(context, enumerable, cardType)
        {
            _presenter = presenter;
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = $"{item.Name.First} {item.Name.Last}";

            if (!string.IsNullOrWhiteSpace(item.Name?.Native))
            {
                holder.DetailPrimary.Visibility = ViewStates.Visible;
                holder.DetailPrimary.Text = item.Name.Native;
            }
            else
            {
                holder.DetailPrimary.Visibility = ViewStates.Gone;
            }

            var altNames = item.Name?.Alternative?.Where(x => !string.IsNullOrWhiteSpace(x));
            if (altNames?.Any() == true)
            {
                holder.DetailSecondary.Visibility = ViewStates.Visible;
                holder.DetailSecondary.Text = $"Also known as: {string.Join(", ", item.Name.Alternative)}";
            }
            else
            {
                holder.DetailSecondary.Visibility = ViewStates.Gone;
            }

            Context.LoadImage(holder.Image, item.Image?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;

            if (item.IsFavourite)
            {
                holder.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24dp);
                holder.ButtonIcon.ImageTintList = FavoriteIconColor;
            }
            else
            {
                holder.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_border_white_24dp);
                holder.ButtonIcon.ImageTintList = DefaultIconColor;
            }

            holder.Button.SetTag(Resource.Id.Object_Position, position);
            holder.Button.Click -= ButtonClick;
            holder.Button.Click += ButtonClick;
        }

        private void RowClick(object sender, EventArgs e)
        {
            //var senderView = sender as View;
            //var characterPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            //var character = Items[characterPos];

            //CharacterActivity.StartActivity(Context, character.Id);
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var characterPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var character = Items[characterPos];

            senderView.Clickable = false;
            character = await _presenter.ToggleCharacterFavorite(character);
            NotifyItemChanged(characterPos);
            senderView.Clickable = true;
        }
    }
}