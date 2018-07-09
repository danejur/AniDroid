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
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListTabOrderRecyclerAdapter : BaseDraggableRecyclerAdapter<BaseRecyclerAdapter.StableIdItem<KeyValuePair<string, bool>>>
    {
        public MediaListTabOrderRecyclerAdapter(BaseAniDroidActivity context, List<StableIdItem<KeyValuePair<string, bool>>> items) : base(context, items)
        {
        }

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var cardItem = holder as DraggableCardItemViewHolder;
            var item = Items[position];

            cardItem.Name.Text = item.Item.Key;
        }
    }
}