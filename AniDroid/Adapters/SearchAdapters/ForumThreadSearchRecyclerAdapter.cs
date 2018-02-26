using System;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.SearchAdapters
{
    public class ForumThreadSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<ForumThread>
    {
        public ForumThreadSearchRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<ForumThread>> enumerable) : base(context, enumerable, CardType.Horizontal)
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

        private static void RowClick(object sender, EventArgs e)
        {
            // TODO: start forumthread activity here
        }
    }
}