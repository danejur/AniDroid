using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Studio;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StudioAdapters
{
    public class StudioRecyclerAdapter : AniDroidRecyclerAdapter<StudioViewModel, Studio>
    {
        public StudioRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Studio>, IAniListError>> enumerable) : base(context, enumerable,
            RecyclerCardType.Horizontal)
        {
            ClickAction =
                viewModel => StudioActivity.StartActivity(Context, viewModel.Model.Id);
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Name.Text = viewModel.TitleText;
            holder.Button.Visibility = viewModel.Model.IsFavourite ? ViewStates.Visible : ViewStates.Gone;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);
            item.ContainerCard.SetContentPadding(0, 20, 0, 20);
            item.Image.Visibility = item.DetailPrimary.Visibility = item.DetailSecondary.Visibility = ViewStates.Gone;

            return item;
        }
    }
}