using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;

namespace AniDroid.Utils.Extensions
{
    public static class RecyclerViewExtensions
    {
        public static ItemTouchHelper AddDragAndDropSupport(this RecyclerView recyclerView, bool useLongPress = true)
        {
            var helper = new DragItemTouchHelper(new DragItemTouchHelperCallback(useLongPress));
            helper.AttachToRecyclerView(recyclerView);

            return helper;
        }

        private class DragItemTouchHelper : ItemTouchHelper
        {
            protected DragItemTouchHelper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public DragItemTouchHelper(Callback callback) : base(callback)
            {
            }

            public override void StartDrag(RecyclerView.ViewHolder viewHolder)
            {
                base.StartDrag(null);
            }
        }

        private class DragItemTouchHelperCallback : ItemTouchHelper.Callback
        {
            public DragItemTouchHelperCallback(bool useLongPress)
            {
                IsLongPressDragEnabled = useLongPress;
            }

            public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
            {
                return MakeMovementFlags(ItemTouchHelper.Up | ItemTouchHelper.Down, 0);
            }

            public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
            {
                recyclerView.GetAdapter().NotifyItemMoved(viewHolder.AdapterPosition, target.AdapterPosition);
                return true;
            }

            public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
            {
            }

            public override bool IsLongPressDragEnabled { get; }
        }
    }
}