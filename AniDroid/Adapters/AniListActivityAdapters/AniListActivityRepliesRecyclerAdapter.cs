using System;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.AniListObject;
using AniDroid.AniListObject.User;
using AniDroid.Base;

namespace AniDroid.Adapters.AniListActivityAdapters
{
    public class AniListActivityRepliesRecyclerAdapter : AniDroidRecyclerAdapter<AniListActivityReplyViewModel, ActivityReply>
    {
        private readonly IAniListActivityPresenter _presenter;
        private readonly Color _userNameColor;

        public AniListActivityRepliesRecyclerAdapter(BaseAniDroidActivity context, IAniListActivityPresenter presenter,
            List<AniListActivityReplyViewModel> items) : base(context, items, RecyclerCardType.Custom)
        {
            _presenter = presenter;
            _userNameColor = new Color(Context.GetThemedColor(Resource.Attribute.Primary));
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewModel = Items[position];

            if (holder is AniListActivityRecyclerAdapter.AniListActivityViewHolder activityHolder)
            {
                activityHolder.Title.Text = viewModel.TitleText;
                activityHolder.ContentText.TextFormatted = viewModel.DetailFormatted;
                activityHolder.Timestamp.Text = viewModel.TimestampText;
                activityHolder.LikeCount.Text = viewModel.LikeCount;
                activityHolder.LikeIcon.ImageTintList = viewModel.LikeIconColor;
                activityHolder.LikeIcon.SetTag(Resource.Id.Object_Position, position);
                activityHolder.LikeIcon.Click -= IconClick;
                activityHolder.LikeIcon.Click += IconClick;

                Context.LoadImage(activityHolder.Image, viewModel.ImageUri);

                activityHolder.Image.SetTag(Resource.Id.Object_Position, position);
                activityHolder.Image.Click -= ImageClick;
                activityHolder.Image.Click += ImageClick;

                activityHolder.Container.SetTag(Resource.Id.Object_Position, position);
                activityHolder.Container.LongClick -= RowLongClick;
                activityHolder.Container.LongClick += RowLongClick;
            }
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            var holder = new AniListActivityRecyclerAdapter.AniListActivityViewHolder(
                Context.LayoutInflater.Inflate(Resource.Layout.View_AniListActivityItem, parent, false))
            {
                ReplyButton = {Visibility = ViewStates.Gone},
                ContentImageContainer = {Visibility = ViewStates.Gone},
                ReplyCountContainer = {Visibility = ViewStates.Gone}
            };

            holder.Title.SetTextColor(_userNameColor);
            holder.Container.SetBackgroundColor(Color.Transparent);

            return holder;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Name.SetTextColor(_userNameColor);
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.Container.SetBackgroundColor(Color.Transparent);
            item.ContainerCard.CardBackgroundColor = ColorStateList.ValueOf(Color.Transparent);
            return item;
        }

        private void ImageClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var userPos = (int) senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[userPos];
            UserActivity.StartActivity(Context, viewModel.Model?.User?.Id ?? 0);
        }

        private async void IconClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var position = (int)senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[position];

            await _presenter.ToggleActivityReplyLikeAsync(viewModel.Model, position);
            viewModel.RecreateViewModel();
            NotifyItemChanged(position);
        }
    }
}