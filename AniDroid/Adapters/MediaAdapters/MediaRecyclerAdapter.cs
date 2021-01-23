using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaRecyclerAdapter : AniDroidRecyclerAdapter<MediaViewModel, Media>
    {
        public UserMediaListOptions UserMediaListOptions { get; set; }

        private int? _cardWidth;

        public MediaRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<Media, MediaViewModel> createViewModelFunc) : base(context, enumerable, cardType, createViewModelFunc)
        {
            SetDefaultClickActions();
        }

        public MediaRecyclerAdapter(BaseAniDroidActivity context, List<MediaViewModel> items, RecyclerCardType cardType)
            : base(context, items, cardType)
        {
            SetDefaultClickActions();
        }

        private void SetDefaultClickActions()
        {
            ClickAction = (viewModel, position) =>
                MediaActivity.StartActivity(Context, viewModel.Model.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);

            LongClickAction = (viewModel, position) =>
                Context.DisplaySnackbarMessage(viewModel.Model.Title?.UserPreferred, Snackbar.LengthLong);
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Image.SetBackgroundColor(viewModel.ImageColor);

            base.BindCardViewHolder(holder, position);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            if (_cardWidth.HasValue)
            {
                item.Container.LayoutParameters.Width = _cardWidth.Value;
            }

            item.Button.Clickable = false;
            item.ButtonIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
            ImageViewCompat.SetImageTintList(item.ButtonIcon, FavoriteIconColor);

            item.Name.SetSingleLine(false);
            item.Name.SetMaxLines(2);

            return item;
        }

        public void SetHorizontalAdapterCardWidthDip(int dp)
        {
            _cardWidth = LoadingCardWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Context.Resources.DisplayMetrics);
            LoadingCardHeight = ViewGroup.LayoutParams.MatchParent;
            CardColumnCount = 1;
            SetHorizontalOrientation();
        }
    }
}