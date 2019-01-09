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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.UserAdapters
{
    public class AniListNotificationRecyclerAdapter : AniDroidRecyclerAdapter<AniListNotificationViewModel, AniListNotification>
    {
        private readonly int _unreadCount;

        public AniListNotificationRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> enumerable, int unreadCount,
            Func<AniListNotification, AniListNotificationViewModel> createViewModelFunc) : base(context, enumerable,
            RecyclerCardType.Custom, createViewModelFunc)
        {
            _unreadCount = unreadCount;
            CustomCardUseItemDecoration = true;
            ClickAction = viewModel => (viewModel as AniListNotificationViewModel)?.ClickAction?.Invoke();
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as AniListNotificationViewHolder;
            var viewModel = Items[position];

            viewHolder.Text.TextFormatted = viewModel.FormattedTitle;
            viewHolder.Timestamp.Text = viewModel.Timestamp;
            Context.LoadImage(viewHolder.Image, viewModel.ImageUri);

            viewHolder.ItemView.SetTag(Resource.Id.Object_Position, position);
            viewHolder.ItemView.Click -= RowClick;
            viewHolder.ItemView.Click += RowClick;
        }

        public override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return new AniListNotificationViewHolder(
                Context.LayoutInflater.Inflate(Resource.Layout.View_AniListNotificationItem, parent, false));
        }

        public class AniListNotificationViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Image { get; set; }
            public TextView Text { get; set; }
            public TextView Timestamp { get; set; }

            public AniListNotificationViewHolder(View itemView) : base(itemView)
            {
                Image = itemView.FindViewById<ImageView>(Resource.Id.AniListNotification_Image);
                Text = itemView.FindViewById<TextView>(Resource.Id.AniListNotification_Text);
                Timestamp = itemView.FindViewById<TextView>(Resource.Id.AniListNotification_Timestamp);
            }
        }
    }
}