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
using OneOf;

namespace AniDroid.Adapters.AniListActivityAdapters
{
    public class AniListActivityAdapter : BaseRecyclerAdapter
    {
        private const int ProgressBarViewType = -1;

        private readonly IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> _activityEnumerable;
        private readonly IAsyncEnumerator<OneOf<IPagedData<AniListActivity>, IAniListError>> _activityEnumerator;
        private bool _isLazyLoading;
        private readonly string _userNameColorHex;
        private readonly string _actionColorHex;
        private readonly int _userId;
        private readonly Color _defaultIconColor;

        public List<AniListActivity> Items { get; protected set; }

        public AniListActivityAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> enumerable, int userId) : base(context)
        {
            _activityEnumerable = enumerable;
            _activityEnumerator = enumerable.GetEnumerator();
            _userNameColorHex = $"#{Context.GetThemedColor(Resource.Attribute.Primary) & 0xffffff:X6}";
            _actionColorHex = $"#{Context.GetThemedColor(Resource.Attribute.Primary_Dark) & 0xffffff:X6}";
            _userId = userId;
            _defaultIconColor = new Color(context.GetThemedColor(Resource.Attribute.Secondary_Dark));
            Items = new List<AniListActivity> {null};
        }

        private void BindAniListActivityViewHolder(RecyclerView.ViewHolder holder, int position)
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
            //viewHolder.ReplyLikeContainer.Click -= ShowReplyDialog;
            //viewHolder.ReplyLikeContainer.Click += ShowReplyDialog;

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

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (Items[position] != null)
            {
                BindAniListActivityViewHolder(holder, position);
                return;
            }

            if (position < ItemCount - 1 || _isLazyLoading)
            {
                return;
            }

            _isLazyLoading = true;

            var moveNextResult = await _activityEnumerator.MoveNextAsync();

            _activityEnumerator.Current?.Switch((IAniListError error) =>
                    Context.DisplaySnackbarMessage("Error occurred while getting next page of data",
                        Snackbar.LengthLong))
                .Switch(data =>
                {
                    if (!moveNextResult)
                    {
                        return;
                    }

                    AddItems(data.Data, data.PageInfo.HasNextPage);
                });

            RemoveItem(position);

            _isLazyLoading = false;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType != ProgressBarViewType)
            {
                return new AniListActivityViewHolder(
                    Context.LayoutInflater.Inflate(Resource.Layout.View_AniListActivityItem, parent, false));
            }

            var view = Context.LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, parent,
                false);
            var holder = new ProgressBarViewHolder(view);

            return holder;
        }

        public override int ItemCount => Items?.Count ?? 0;

        public bool ReplaceItem(int position, AniListActivity item, bool notify = true)
        {
            if (_isLazyLoading)
            {
                return false;
            }

            if (position >= ItemCount || position < 0)
            {
                return false;
            }

            Items[position] = item;

            if (notify)
            {
                NotifyItemChanged(position);
            }

            return true;
        }

        private void AddItems(IEnumerable<AniListActivity> itemsToAdd, bool hasNextPage)
        {
            var initialAdd = Items.Count == 1 && Items[0] == null;

            if (hasNextPage)
            {
                itemsToAdd = itemsToAdd.Append(null);
            }

            Items.AddRange(itemsToAdd);

            NotifyDataSetChanged();

            if (initialAdd)
            {
                var controller =
                    AnimationUtils.LoadLayoutAnimation(Context, Resource.Animation.Layout_Animation_Falldown);
                RecyclerView.LayoutAnimation = controller;
                RecyclerView.ScheduleLayoutAnimation();
            }
        }

        private void RemoveItem(int position)
        {
            Items.RemoveAt(position);
            NotifyItemRemoved(position);
        }

        public sealed override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);

            recyclerView.AddItemDecoration(new DefaultItemDecoration(Context));
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        }

        public sealed override int GetItemViewType(int position)
        {
            return (Items[position] == null) ? ProgressBarViewType : 0;
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

        private class ProgressBarViewHolder : RecyclerView.ViewHolder
        {
            public ProgressBarViewHolder(View itemView) : base(itemView)
            {
            }
        }
    }
}