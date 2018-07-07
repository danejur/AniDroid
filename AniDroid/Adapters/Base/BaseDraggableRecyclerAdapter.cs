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
    public abstract class BaseDraggableRecyclerAdapter<T> : BaseRecyclerAdapter<T>, IDraggableItemAdapter
    {
        protected BaseDraggableRecyclerAdapter(BaseAniDroidActivity context, List<T> items) : base(context, items, RecyclerCardType.FlatHorizontal)
        {
            HasStableIds = true;
        }

        public bool OnCheckCanDrop(int p0, int p1)
        {
            return true;
        }

        public bool OnCheckCanStartDrag(Object p0, int p1, int p2, int p3)
        {
            return true;
        }

        public ItemDraggableRange OnGetItemDraggableRange(Object p0, int p1)
        {
            return null;
        }

        public void OnItemDragFinished(int p0, int p1, bool p2)
        {
            NotifyDataSetChanged();
        }

        public void OnItemDragStarted(int p0)
        {
            NotifyDataSetChanged();
        }

        public void OnMoveItem(int p0, int p1)
        {
            NotifyDataSetChanged();
        }
    }
}