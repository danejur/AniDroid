using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using AniDroid.Base;
using AniDroid.Utils.Extensions;

namespace AniDroid.Adapters.Base
{
    public abstract class BaseDraggableRecyclerAdapter<T> : BaseRecyclerAdapter<T> where T : BaseRecyclerAdapter.IStableIdItem
    {
        private readonly ItemTouchHelper _touchHelper;

        protected BaseDraggableRecyclerAdapter(BaseAniDroidActivity context, List<T> items) : base(context, items, RecyclerCardType.Custom)
        {
            HasStableIds = true;

            _touchHelper = RecyclerView.AddDragAndDropSupport(false);
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

        public override void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dragHolder = holder as DraggableCardItemViewHolder;

            if (dragHolder == null)
            {
                return;
            }


            dragHolder.DragHandle.SetTag(Resource.Id.Object_Position, position);
            //dragHolder.DragHandle.Touch -= StartDrag;
            //dragHolder.DragHandle.Touch += StartDrag;

            //dragHolder.DragHandle.Touch += (sender, e) =>
            //{
            //    if (e.Event?.Action == MotionEventActions.Down)
            //    {
            //        var view = sender as View;

            //        if (view == null)
            //        {
            //            return;
            //        }


            //        _touchHelper.StartDrag(dragHolder);
            //    }
            //};
        }

        //private void StartDrag(object sender, View.TouchEventArgs e)
        //{
        //    if (e.Event?.Action == MotionEventActions.Down)
        //    {
        //        var view = sender as View;

        //        if (view == null)
        //        {
        //            return;
        //        }

        //        var position = (int) view.GetTag(Resource.Id.Object_Position);
        //        var holder = RecyclerView.FindV(position);

        //        _touchHelper.StartDrag(holder);
        //    }
        //}

        public class DraggableCardItemViewHolder : RecyclerView.ViewHolder
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