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
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;
using OneOf;

namespace AniDroid.Adapters.TorrentAdapters
{
    public class NyaaSiSearchRecyclerAdapter : LazyLoadingRecyclerViewAdapter<NyaaSiSearchResult>
    {
        public NyaaSiSearchRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> enumerable) : base(context,
            enumerable, RecyclerCardType.FlatHorizontal)
        {
        }

        public NyaaSiSearchRecyclerAdapter(BaseAniDroidActivity context,
            LazyLoadingRecyclerViewAdapter<NyaaSiSearchResult> adapter) : base(context, adapter)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Title;

            // TODO: finish this
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;

            return item;
        }
    }
}