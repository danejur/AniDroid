using System;
using System.Collections.Generic;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffEdgeRecyclerAdapter : AniDroidRecyclerAdapter<StaffEdgeViewModel, Staff.Edge>
    {
        public StaffEdgeRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Staff.Edge>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<Staff.Edge, StaffEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType,
            createViewModelFunc)
        {
            SetupDefaultClickActions();
        }

        private void SetupDefaultClickActions()
        {
            ClickAction = (viewModel, position) =>
                StaffActivity.StartActivity(Context, viewModel.Model.Node.Id,
                    BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}