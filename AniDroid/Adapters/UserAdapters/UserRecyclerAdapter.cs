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
        public UserRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> enumerable, RecyclerCardType cardType) : base(context, enumerable, cardType)
        {
        }

        public UserRecyclerAdapter(BaseAniDroidActivity context, List<UserViewModel> list, RecyclerCardType cardType) : base(context, list, cardType)
        {
        }

        public override Action<AniDroidAdapterViewModel<User>> ClickAction =>
            viewModel => UserActivity.StartActivity(Context, viewModel.Model.Id);

        public override Action<AniDroidAdapterViewModel<User>> LongClickAction { get; }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Name.Text = viewModel.TitleText;
            holder.DetailPrimary.Text = viewModel.DetailPrimaryText;
            Context.LoadImage(holder.Image, viewModel.ImageUri);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            item.DetailSecondary.Visibility = ViewStates.Gone;
            return item;
        }
    }
}