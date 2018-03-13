using System;
using Android.Content;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.SearchAdapters
{
    public class ForumThreadSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<ForumThread>
    {
        public ForumThreadSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> enumerable) : base(context, enumerable, CardType.Horizontal)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title;
            holder.DetailPrimary.Text = $"Created {item.GetDateTimeOffset(item.CreatedAt):MM/dd/yyyy HH:mm:ss}";
            holder.DetailSecondary.Text = $"Replies: {item.ReplyCount}     Likes: {item.Likes?.Count ?? 0}";
            Context.LoadImage(holder.Image, item.User?.Avatar?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            // TODO: start forumthread activity here
            var senderView = sender as View;
            var itemPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var item = Items[itemPos];

            var intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(item.SiteUrl));
            Context.StartActivity(intent);
        }
    }
}