using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffRecyclerAdapter : AniDroidRecyclerAdapter<StaffViewModel, Staff>
    {
        public StaffRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Staff>, IAniListError>> enumerable, RecyclerCardType cardType) : base(
            context, enumerable, cardType)
        {
            SetDefaultClickActions();
        }

        public StaffRecyclerAdapter(BaseAniDroidActivity context, List<StaffViewModel> list, RecyclerCardType cardType)
            : base(context, list, cardType)
        {
            SetDefaultClickActions();
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Name.Text = viewModel.TitleText;
            holder.DetailPrimary.Text = viewModel.DetailPrimaryText;
            holder.DetailSecondary.Text = viewModel.DetailSecondaryText;
            Context.LoadImage(holder.Image, viewModel.ImageUri);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }

        private void SetDefaultClickActions()
        {
            ClickAction = viewModel =>
                StaffActivity.StartActivity(Context, viewModel.Model.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}