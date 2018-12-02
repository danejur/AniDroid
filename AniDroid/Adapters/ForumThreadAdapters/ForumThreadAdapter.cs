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
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.ForumThreadAdapters
{
    public class ForumThreadAdapter : LazyLoadingAniDroidRecyclerAdapter<ForumThreadViewModel, ForumThread>
    {
        public ForumThreadAdapter(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> enumerable) : base(context, enumerable, RecyclerCardType.Horizontal)
        {
        }

        public override Action<AniDroidAdapterViewModel<ForumThread>> ClickAction => viewModel =>
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(viewModel.Model.SiteUrl));
            Context.StartActivity(intent);
        };

        public override Action<AniDroidAdapterViewModel<ForumThread>> LongClickAction { get; }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var viewModel = Items[position];

            holder.Name.Text = viewModel.TitleText;
            holder.DetailPrimary.Text = viewModel.DetailPrimaryText;
            holder.DetailSecondary.Text = viewModel.DetailSecondaryText;
            Context.LoadImage(holder.Image, viewModel.ImageUri);
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }
    }
}