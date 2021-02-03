using System;
using System.Collections.Generic;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.StudioModels;
using AniDroid.AniListObject.Studio;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StudioAdapters
{
    public class StudioEdgeRecyclerAdapter : AniDroidRecyclerAdapter<StudioEdgeViewModel, StudioEdge>
    {
        public StudioEdgeRecyclerAdapter(BaseAniDroidActivity context, List<StudioEdgeViewModel> items) : base(context,
            items, RecyclerCardType.Horizontal)
        {
            ClickAction = (viewModel, position) => StudioActivity.StartActivity(Context, viewModel.Model?.Node?.Id ?? 0,
                BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

        public StudioEdgeRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<StudioEdge>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<StudioEdge, StudioEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType,
            createViewModelFunc)
        {
            ClickAction = (viewModel, position) => StudioActivity.StartActivity(Context, viewModel.Model?.Node?.Id ?? 0,
                BaseAniDroidActivity.ObjectBrowseRequestCode);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.ContainerCard.SetContentPadding(20, 20, 20, 20);

            return item;
        }
    }
}