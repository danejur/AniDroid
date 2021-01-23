using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffEdgeRecyclerAdapter : AniDroidRecyclerAdapter<StaffEdgeViewModel, StaffEdge>
    {
        public StaffEdgeRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<StaffEdge, StaffEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType,
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