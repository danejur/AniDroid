using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.AniListObject.User;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Home;
using OneOf;

namespace AniDroid.Adapters.AniListActivityAdapters
{
    public class AniListActivityRecyclerAdapter : LazyLoadingRecyclerViewAdapter<AniListActivity>
    {
        private readonly HomePresenter _presenter;
        private readonly string _userNameColorHex;
        private readonly string _actionColorHex;
        private readonly int _userId;
        private readonly Color _defaultIconColor;

        public AniListActivityRecyclerAdapter(BaseAniDroidActivity context, HomePresenter presenter,
            IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> enumerable, int userId) : base(context, enumerable, CardType.Custom)
        {
            _presenter = presenter;
            _userNameColorHex = $"#{Context.GetThemedColor(Resource.Attribute.Primary) & 0xffffff:X6}";
            _actionColorHex = $"#{Context.GetThemedColor(Resource.Attribute.Primary_Dark) & 0xffffff:X6}";
            _userId = userId;
            _defaultIconColor = new Color(context.GetThemedColor(Resource.Attribute.Secondary_Dark));
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as AniListActivityViewHolder;
            var item = Items[position];

            viewHolder.Timestamp.Text = item.GetAgeString(item.CreatedAt);

            if (item.ReplyCount > 0)
            {
                viewHolder.ReplyCountContainer.Visibility = ViewStates.Visible;
                viewHolder.ReplyCount.Text = item.ReplyCount.ToString();
            }
            else
            {
                viewHolder.ReplyCountContainer.Visibility = ViewStates.Gone;
            }

            viewHolder.LikeCount.Text = item.Likes?.Count.ToString();
            viewHolder.LikeIcon.ImageTintList = ColorStateList.ValueOf(item.Likes?.Any(x => x.Id == _userId) == true ? Color.Crimson : _defaultIconColor);
            viewHolder.ReplyLikeContainer.SetTag(Resource.Id.Object_Position, position);
            viewHolder.ReplyLikeContainer.Click -= ShowReplyDialog;
            viewHolder.ReplyLikeContainer.Click += ShowReplyDialog;

            viewHolder.Image.SetTag(Resource.Id.Object_Position, position);
            viewHolder.Image.Click -= ImageClick;
            viewHolder.Image.Click += ImageClick;

            if (item.Type == AniListActivity.ActivityType.Text)
            {
                BindTextActivityViewHolder(viewHolder, item);
            }
            else if (item.Type == AniListActivity.ActivityType.Message)
            {
                BindMessageActivityViewHolder(viewHolder, item);
            }
            else if (item.Type == AniListActivity.ActivityType.AnimeList ||
                     item.Type == AniListActivity.ActivityType.MangaList)
            {
                BindListActivityViewHolder(viewHolder, item);
            }
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return new AniListActivityViewHolder(
                Context.LayoutInflater.Inflate(Resource.Layout.View_AniListActivityItem, parent, false));
        }


        private void BindTextActivityViewHolder(AniListActivityViewHolder viewHolder, AniListActivity item)
        {
            viewHolder.Title.TextFormatted = BaseAniDroidActivity.FromHtml($"<b><font color='{_userNameColorHex}'>{item.User.Name}</font></b>");
            viewHolder.ContentText.TextFormatted = BaseAniDroidActivity.FromHtml(item.Text);
            viewHolder.ContentText.Visibility = ViewStates.Visible;
            viewHolder.ContentImageContainer.Visibility = ViewStates.Gone;

            Context.LoadImage(viewHolder.Image, item.User.Avatar.Large);
        }

        private void BindMessageActivityViewHolder(AniListActivityViewHolder viewHolder, AniListActivity item)
        {
            viewHolder.Title.TextFormatted = BaseAniDroidActivity.FromHtml($"<b><font color='{_userNameColorHex}'>{item.Messenger.Name}</font></b>");
            viewHolder.ContentText.TextFormatted = BaseAniDroidActivity.FromHtml(item.Message);
            viewHolder.ContentText.Visibility = ViewStates.Visible;
            viewHolder.ContentImageContainer.Visibility = ViewStates.Gone;

            Context.LoadImage(viewHolder.Image, item.Messenger.Avatar.Large);
        }

        private void BindListActivityViewHolder(AniListActivityViewHolder viewHolder, AniListActivity item)
        {
            viewHolder.Title.TextFormatted = BaseAniDroidActivity.FromHtml($"<b><font color='{_userNameColorHex}'>{item.User.Name}</font></b> {item.Status} {(!string.IsNullOrWhiteSpace(item.Progress) ? $"{item.Progress} of" : "")} <b><font color='{_actionColorHex}'>{item.Media.Title.UserPreferred}</font></b>");
            viewHolder.ContentText.Visibility = ViewStates.Gone;
            viewHolder.ContentImageContainer.Visibility = ViewStates.Visible;
            viewHolder.ContentImageContainer.RemoveAllViews();

            Context.LoadImage(viewHolder.Image, item.Media.CoverImage.Large);
        }

        private void ImageClick(object sender, EventArgs e)
        {
            var image = sender as ImageView;
            var position = (int)image.GetTag(Resource.Id.Object_Position);
            var item = Items[position];

            if (item.Type == AniListActivity.ActivityType.Text)
            {
                UserActivity.StartActivity(Context, item.User.Id);
            }
            else if (item.Type == AniListActivity.ActivityType.Message)
            {
                UserActivity.StartActivity(Context, item.Messenger.Id);
            }
            else if (item.Type == AniListActivity.ActivityType.AnimeList || item.Type == AniListActivity.ActivityType.MangaList)
            {
                MediaActivity.StartActivity(Context, item.Media.Id);
            }
        }

        private void ShowReplyDialog(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var activityPosition = (int)senderView.GetTag(Resource.Id.Object_Position);
            var activity = Items[activityPosition];

            AniListActivityRepliesDialog.Create(Context, activity, _userId, PostReply, ToggleLikeActivity);
        }

        private async void ToggleLikeActivity(int activityId)
        {
            var activityItemPosition = Items.FindIndex(x => x.Id == activityId);
            var activityItem = Items[activityItemPosition];
            Items[activityItemPosition] = null;
            NotifyItemChanged(activityItemPosition);

            await _presenter.ToggleLike(activityItem, activityItemPosition);
        }

        private async void PostReply(int activityId, string text)
        {
            var activityItemPosition = Items.FindIndex(x => x.Id == activityId);
            var activityItem = Items[activityItemPosition];
            Items[activityItemPosition] = null;
            NotifyItemChanged(activityItemPosition);

            await _presenter.PostActivityReply(activityItem, activityItemPosition, text);
        }

        public class AniListActivityViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; set; }
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView Timestamp { get; set; }
            public TextView ContentText { get; set; }
            public LinearLayout ContentImageContainer { get; set; }
            public View ReplyContainer { get; set; }
            public View ReplyCountContainer { get; set; }
            public TextView ReplyCount { get; set; }
            public View ReplyLikeContainer { get; set; }
            public TextView LikeCount { get; set; }
            public ImageView LikeIcon { get; set; }

            public AniListActivityViewHolder(View view) : base(view)
            {
                Container = view.FindViewById(Resource.Id.AniListActivity_Container);
                Image = view.FindViewById<ImageView>(Resource.Id.AniListActivity_Image);
                Title = view.FindViewById<TextView>(Resource.Id.AniListActivity_Title);
                Timestamp = view.FindViewById<TextView>(Resource.Id.AniListActivity_Timestamp);
                ContentText = view.FindViewById<TextView>(Resource.Id.AniListActivity_ContentText);
                ContentImageContainer = view.FindViewById<LinearLayout>(Resource.Id.AniListActivity_ContentImageContainer);
                ReplyContainer = view.FindViewById(Resource.Id.AniListActivity_ReplyContainer);
                ReplyCountContainer = view.FindViewById(Resource.Id.AniListActivity_ReplyCountContainer);
                ReplyCount = view.FindViewById<TextView>(Resource.Id.AniListActivity_ReplyCount);
                ReplyLikeContainer = view.FindViewById(Resource.Id.AniListActivity_ReplyLikeContainer);
                LikeCount = view.FindViewById<TextView>(Resource.Id.AniListActivity_LikeCount);
                LikeIcon = view.FindViewById<ImageView>(Resource.Id.AniListActivity_LikeIcon);
            }
        }
    }
}