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

namespace AniDroid.Base
{
    public abstract class BaseRecyclerAdapter<T> : BaseRecyclerAdapter
    {
        public List<T> Items { get; protected set; }
        public sealed override int ItemCount => Items.Count;

        protected BaseRecyclerAdapter(BaseAniDroidActivity context, List<T> items) : base(context)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public void AddItems(params T[] items)
        {
            Items.AddRange(items);
            NotifyDataSetChanged();
        }

        public void AddItems(IEnumerable<T> items)
        {
            Items.AddRange(items);
            NotifyDataSetChanged();
        }
    }

    public abstract class BaseRecyclerAdapter : RecyclerView.Adapter
    {
        protected RecyclerView RecyclerView { get; private set; }
        protected BaseAniDroidActivity Context { get; }

        protected BaseRecyclerAdapter(BaseAniDroidActivity context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            RecyclerView = recyclerView;
        }
    }
}