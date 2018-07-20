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
using AniDroid.Base;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;
using Object = Java.Lang.Object;

namespace AniDroid.Adapters.Base
{
    public abstract class BaseDraggableRecyclerAdapter<T> : BaseRecyclerAdapter<T>, IDraggableItemAdapter where T : BaseRecyclerAdapter.IStableIdItem
    {
        protected BaseDraggableRecyclerAdapter(BaseAniDroidActivity context, List<T> items) : base(context, items, RecyclerCardType.Custom)
        {
            HasStableIds = true;
        }

        public bool OnCheckCanDrop(int p0, int p1)
        {
            return true;
        }

        public bool OnCheckCanStartDrag(Object p0, int p1, int p2, int p3)
        {
            var holder = p0 as DraggableCardItemViewHolder;

            var dragHandle = holder?.DragHandle;

            var handleWidth = dragHandle.Width;
            var handleHeight = dragHandle.Height;
            var handleLeft = dragHandle.Left;
            var handleTop = dragHandle.Top;

            return (p2 >= handleLeft) && (p2 < handleLeft + handleWidth) &&
                   (p3 >= handleTop) && (p3 < handleTop + handleHeight);
        }

        public ItemDraggableRange OnGetItemDraggableRange(Object p0, int p1)
        {
            return null;
        }

        public void OnItemDragFinished(int p0, int p1, bool p2)
        {
        }

        public void OnItemDragStarted(int p0)
        {
        }

        public void OnMoveItem(int p0, int p1)
        {
            MoveItem(p0, p1);
        }

        public override long GetItemId(int position)
        {
            return Items[position].StableId;
        }

        public sealed override RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return new DraggableCardItemViewHolder(
                Context.LayoutInflater.Inflate(Resource.Layout.View_DraggableCardItem, parent, false));
        }

        public class DraggableCardItemViewHolder : AbstractDraggableItemViewHolder
        {
            public TextView Name { get; set; }
            public AppCompatCheckBox Checkbox { get; set; }
            public View DragHandle { get; set; }

            public DraggableCardItemViewHolder(View itemView) : base(itemView)
            {
                DragHandle = itemView.FindViewById(Resource.Id.DraggableCardItem_Handle);
                Name = itemView.FindViewById<TextView>(Resource.Id.DraggableCardItem_Name);
                Checkbox = itemView.FindViewById<AppCompatCheckBox>(Resource.Id.DraggableCardItem_Checkbox);
            }
        }
    }
}