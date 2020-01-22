﻿using System;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.RecommendationAdapters
{
    public class RecommendationEdgeRecyclerAdapter : AniDroidRecyclerAdapter<RecommendationEdgeViewModel,Recommendation.Edge>
    {
        public RecommendationEdgeRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<Recommendation.Edge>, IAniListError>> enumerable, RecyclerCardType cardType, Func<Recommendation.Edge, RecommendationEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType, createViewModelFunc)
        {
            SetDefaultClickActions();
        }

        private void SetDefaultClickActions()
        {
            ClickAction = viewModel =>
                MediaActivity.StartActivity(Context, viewModel.Model.Node.MediaRecommendation.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);

            LongClickAction = viewModel =>
                Context.DisplaySnackbarMessage(viewModel.Model.Node.MediaRecommendation.Title?.UserPreferred, Snackbar.LengthLong);
        }
    }
}