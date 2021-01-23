using System;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.RecommendationModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.RecommendationAdapters
{
    public class RecommendationEdgeRecyclerAdapter : AniDroidRecyclerAdapter<RecommendationEdgeViewModel,ConnectionEdge<Recommendation>>
    {
        public RecommendationEdgeRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<ConnectionEdge<Recommendation>>, IAniListError>> enumerable, RecyclerCardType cardType, Func<ConnectionEdge<Recommendation>, RecommendationEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType, createViewModelFunc)
        {
            SetDefaultClickActions();

            ValidateItemFunc = rec => rec.Node?.MediaRecommendation != null;
        }

        private void SetDefaultClickActions()
        {
            ClickAction = (viewModel, position) =>
                MediaActivity.StartActivity(Context, viewModel.Model.Node.MediaRecommendation.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);

            LongClickAction = (viewModel, position) =>
                Context.DisplaySnackbarMessage(viewModel.Model.Node.MediaRecommendation.Title?.UserPreferred, Snackbar.LengthLong);
        }
    }
}