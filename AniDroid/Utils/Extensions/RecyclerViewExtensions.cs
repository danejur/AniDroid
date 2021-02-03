using Android.Runtime;
using System;
using AndroidX.RecyclerView.Widget;

namespace AniDroid.Utils.Extensions
{
    public static class RecyclerViewExtensions
    {
        public static ItemTouchHelper AddDragAndDropSupport(this RecyclerView recyclerView, bool useLongPress = true)
        {
            var helper = new ItemTouchHelper(new DragItemTouchHelperCallback(useLongPress));
            helper.AttachToRecyclerView(recyclerView);

            return helper;
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