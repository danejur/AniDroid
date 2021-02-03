using System;
using System.Collections.Generic;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using Google.Android.Material.Snackbar;
using OneOf;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaEdgeRecyclerAdapter : AniDroidRecyclerAdapter<MediaEdgeViewModel, MediaEdge>
    {
        public MediaEdgeRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<MediaEdge, MediaEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType,
            createViewModelFunc)
        {
            SetDefaultClickActions();
        }

        public MediaEdgeRecyclerAdapter(BaseAniDroidActivity context, List<MediaEdgeViewModel> items,
            RecyclerCardType cardType) : base(context, items, cardType)
        {
            SetDefaultClickActions();
        }

        private void SetDefaultClickActions()
        {
            ClickAction = (viewModel, position) =>
                MediaActivity.StartActivity(Context, viewModel.Model?.Node?.Id ?? 0, BaseAniDroidActivity.ObjectBrowseRequestCode);

            LongClickAction = (viewModel, position) =>
                Context.DisplaySnackbarMessage(viewModel.Model?.Node?.Title?.UserPreferred, Snackbar.LengthLong);
        }
    }
}