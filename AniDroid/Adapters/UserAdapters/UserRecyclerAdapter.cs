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
using AniDroid.AniListObject.User;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.UserAdapters
{
    public class UserRecyclerAdapter : AniDroidRecyclerAdapter<UserViewModel, User>
    {
        public UserRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<User, UserViewModel> createViewModelFunc) : base(context, enumerable, cardType, createViewModelFunc)
        {
            SetDefaultClickActions();
        }

        public UserRecyclerAdapter(BaseAniDroidActivity context, List<UserViewModel> items, RecyclerCardType cardType,
            Func<User, UserViewModel> createViewModelFunc) : base(context, items, cardType, createViewModelFunc)
        {
            SetDefaultClickActions();
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private void SetDefaultClickActions()
        {
            ClickAction =
                viewModel => UserActivity.StartActivity(Context, viewModel.Model.Id);
        }
    }
}