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
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaRecyclerAdapter : AniDroidRecyclerAdapter<MediaViewModel, Media>
    {
        public User.UserMediaListOptions UserMediaListOptions { get; set; }

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
            ClickAction = viewModel =>
                MediaActivity.StartActivity(Context, viewModel.Model.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);

            LongClickAction = viewModel =>
                Context.DisplaySnackbarMessage(viewModel.Model.Title?.UserPreferred, Snackbar.LengthLong);
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Image.SetBackgroundColor(viewModel.ImageColor);
            Context.LoadImage(holder.Image, viewModel.ImageUri ?? "", false);

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

        //private static MediaViewModel.MediaDetailType GetDetailType(Media.MediaSort sort)
        //{
        //    if (Media.MediaSort.Popularity == sort || Media.MediaSort.PopularityDesc == sort)
        //    {
        //        return MediaViewModel.MediaDetailType.AverageRatingPopularity
        //    }
        //    else if (Media.MediaSort.Score == sort || Media.MediaSort.ScoreDesc == sortsortType)
        //    {
        //        retString = $"Score: {item.AverageScore}%";
        //    }
        //}
    }
}