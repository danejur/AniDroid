using System;
using System.Collections.Generic;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.UserModels;
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

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }

        private void SetDefaultClickActions()
        {
            ClickAction =
                (viewModel, position) => UserActivity.StartActivity(Context, viewModel.Model.Id);
        }
    }
}