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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.AniListActivityAdapters
{
    public class AniListNotificationRecyclerAdapter : LazyLoadingRecyclerViewAdapter<AniListNotification>
    {
        private readonly string _accentColorHex;

        public AniListNotificationRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> enumerable) : base(context, enumerable, RecyclerCardType.Custom)
        {
            _accentColorHex = $"#{Context.GetThemedColor(Resource.Attribute.Primary) & 0xffffff:X6}";
            CustomCardUseItemDecoration = true;
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as AniListNotificationViewHolder;
            var item = Items[position];

            viewHolder.Text.TextFormatted = BaseAniDroidActivity.FromHtml(item.GetNotificationHtml(_accentColorHex));
            viewHolder.Timestamp.Text = item.GetAgeString(item.CreatedAt);
            Context.LoadImage(viewHolder.Image, item.GetImageUri());

            viewHolder.ItemView.SetTag(Resource.Id.Object_Position, position);
            viewHolder.ItemView.Click -= RowClick;
            viewHolder.ItemView.Click += RowClick;
        }

        private void RowClick(object sender, EventArgs eventArgs)
        {
            var senderView = sender as View;
            var itemPos = (int) senderView.GetTag(Resource.Id.Object_Position);
            var item = Items[itemPos];

            if (item.Type == AniListNotification.NotificationType.Airing)
            {
                MediaActivity.StartActivity(Context, item.Media.Id);
            }

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