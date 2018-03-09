using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using Android.Util;
using System.Threading.Tasks;
using Android.Support.Annotation;
using Android.Views.Animations;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.Base
{
    public abstract class LazyLoadingRecyclerViewAdapter<T> : BaseRecyclerAdapter<T> where T : class
    {
        private readonly IAsyncEnumerable<IPagedData<T>> _asyncEnumerable;
        private IAsyncEnumerator<IPagedData<T>> _asyncEnumerator;
        private bool _isLazyLoading;
        private bool _dataLoaded;

        protected LazyLoadingRecyclerViewAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<T>> enumerable, CardType cardType, int verticalCardColumns = 3) : base(context, new List<T> { null }, cardType, verticalCardColumns)
        {
            _asyncEnumerable = enumerable;
            _asyncEnumerator = enumerable.GetEnumerator();
        }

        public void ResetAdapter()
        {
            RemoveAllItems();
            AddItems(null);
            _asyncEnumerator = _asyncEnumerable.GetEnumerator();
        }

        public sealed override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (Items[position] != null)
            {
                base.OnBindViewHolder(holder, position);
                return;
            }

            if (position < ItemCount - 1 || _isLazyLoading)
            {
                return;
            }

            _isLazyLoading = true;

            if (await _asyncEnumerator.MoveNextAsync())
            {
                if (!_dataLoaded)
                {
                    DataLoaded?.Invoke(RecyclerView, _asyncEnumerator.Current.PageInfo.Total > 0);
                    _dataLoaded = true;
                }

                AddItems(_asyncEnumerator.Current.Data, _asyncEnumerator.Current.PageInfo.HasNextPage);
            }

            RemoveItem(position);

            _isLazyLoading = false;
        }

        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType != ProgressBarViewType)
            {
                return base.OnCreateViewHolder(parent, viewType);
            }

            var view = Context.LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, parent, false);
            var holder = new ProgressBarViewHolder(view);

            //if (BackgroundColorResource.HasValue)
            //{
            //    holder.Container.SetBackgroundResource(BackgroundColorResource.Value);
            //}

            return holder;
        }

        public override bool InsertItem(int position, T item, bool notify = true)
        {
            return !_isLazyLoading && base.InsertItem(position, item, notify);
        }

        public override bool ReplaceItem(int position, T item, bool notify = true)
        {
            return !_isLazyLoading && base.ReplaceItem(position, item, notify);
        }

        public void AddItems(IEnumerable<T> itemsToAdd, bool hasNextPage)
        {
            var initialAdd = Items.Count == 1 && Items[0] == null;

            if (hasNextPage)
            {
                itemsToAdd = itemsToAdd.Append(null);
            }

            Items.AddRange(itemsToAdd);

            NotifyDataSetChanged();

            if (initialAdd)
            {
                var controller = AnimationUtils.LoadLayoutAnimation(Context, Resource.Animation.Layout_Animation_Falldown);
                RecyclerView.LayoutAnimation = controller;
                RecyclerView.ScheduleLayoutAnimation();
            }
        }

        public sealed override int GetItemViewType(int position)
        {
            return (Items[position] == null) ? ProgressBarViewType : 0;
        }

        private class ProgressBarViewHolder : RecyclerView.ViewHolder
        {
            public ProgressBarViewHolder(View itemView) : base(itemView)
            {
            }
        }

        public event EventHandler<bool> DataLoaded;

        #region Constants

        private const int ProgressBarViewType = -1;

        #endregion
    }
}