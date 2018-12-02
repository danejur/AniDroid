using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.ViewModels;
using AniDroid.Base;

namespace AniDroid.Adapters.Base
{
    public abstract class AniDroidRecyclerAdapter<T, TModel> : BaseRecyclerAdapter where T : AniDroidAdapterViewModel<TModel> where TModel : class
    {
        // TODO: if we refactor all adapters, this class should take the place of BaseRecyclerAdapter<T>

        private int _orientation = LinearLayoutManager.Vertical;
        private RecyclerView.ItemDecoration _decoration;
        protected readonly ColorStateList DefaultIconColor;
        protected readonly ColorStateList FavoriteIconColor;
        protected bool CustomCardUseItemDecoration;
        public RecyclerCardType CardType { get; protected set; }
        public int CardColumns { get; protected set; }
        public List<T> Items { get; protected set; }
        public sealed override int ItemCount => Items.Count;

        public abstract Action<AniDroidAdapterViewModel<TModel>> ClickAction { get; }
        public abstract Action<AniDroidAdapterViewModel<TModel>> LongClickAction { get; }

        protected AniDroidRecyclerAdapter(BaseAniDroidActivity context, List<T> items, RecyclerCardType cardType, int verticalCardColumns = -1) : base(context)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            CardType = cardType;
            CardColumns = verticalCardColumns;
            DefaultIconColor = ColorStateList.ValueOf(new Color(context.GetThemedColor(Resource.Attribute.Secondary_Dark)));
            FavoriteIconColor = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(context, Resource.Color.Favorite_Red)));
        }

        protected void SetHorizontalOrientation()
        {
            _orientation = LinearLayoutManager.Horizontal;
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

        public void MoveItem(int positionFrom, int positionTo)
        {
            var item = Items[positionFrom];
            Items.RemoveAt(positionFrom);

            if (positionTo >= ItemCount)
            {
                AddItems(item);
            }
            else
            {
                InsertItem(positionTo, item);
            }
        }

        public void RemoveItem(int position)
        {
            Items.RemoveAt(position);
            NotifyItemRemoved(position);
        }

        public void RemoveAllItems()
        {
            Items.Clear();
            NotifyDataSetChanged();
        }

        public virtual bool InsertItem(int position, T item, bool notify = true)
        {
            if (position >= ItemCount || position < 0) return false;
            Items.Insert(position, item);

            if (notify)
            {
                NotifyDataSetChanged();
            }

            return true;
        }

        public virtual bool ReplaceItem(int position, T item, bool notify = true)
        {
            if (position >= ItemCount || position < 0) return false;
            Items[position] = item;

            if (notify)
            {
                NotifyItemChanged(position);
            }

            return true;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (CardType == RecyclerCardType.Custom)
            {
                return CreateCustomViewHolder(parent, viewType);
            }

            var layoutResource = Resource.Layout.View_CardItem_Horizontal;

            switch (CardType)
            {
                case RecyclerCardType.FlatHorizontal:
                    layoutResource = Resource.Layout.View_CardItem_FlatHorizontal;
                    break;
                case RecyclerCardType.Vertical:
                    layoutResource = Resource.Layout.View_CardItem_Vertical;
                    break;
            }

            return SetupCardItemViewHolder(new CardItem(Context.LayoutInflater.Inflate(layoutResource, parent, false)));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (CardType == RecyclerCardType.Custom)
            {
                BindCustomViewHolder(holder, position);
            }
            else
            {
                var cardHolder = holder as CardItem;

                BindCardViewHolder(cardHolder, position);
                cardHolder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
                cardHolder.ContainerCard.Click -= RowClick;
                cardHolder.ContainerCard.Click += RowClick;
                cardHolder.ContainerCard.LongClick -= RowLongClick;
                cardHolder.ContainerCard.LongClick += RowLongClick;
            }
        }

        public sealed override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);

            switch (CardType)
            {
                case RecyclerCardType.FlatHorizontal:
                    _decoration = new DefaultItemDecoration(Context);
                    recyclerView.AddItemDecoration(_decoration);
                    recyclerView.SetLayoutManager(new LinearLayoutManager(Context, _orientation, false));
                    break;
                case RecyclerCardType.Horizontal:
                    recyclerView.SetLayoutManager(new LinearLayoutManager(Context, _orientation, false));
                    break;
                case RecyclerCardType.Vertical:
                    recyclerView.SetLayoutManager(new GridLayoutManager(Context, CalculateCardColumns(Context, CardColumns), _orientation, false));
                    break;
                case RecyclerCardType.Custom:
                    if (CustomCardUseItemDecoration)
                    {
                        _decoration = new DefaultItemDecoration(Context);
                        recyclerView.AddItemDecoration(_decoration);
                    }
                    recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
                    break;
            }
        }

        public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
        {
            base.OnDetachedFromRecyclerView(recyclerView);
            if (_decoration != null)
            {
                recyclerView.RemoveItemDecoration(_decoration);
            }
        }

        public void RefreshAdapter()
        {
            OnAttachedToRecyclerView(RecyclerView);
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var mediaPos = (int) senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            ClickAction?.Invoke(viewModel);
        }

        private void RowLongClick(object sender, View.LongClickEventArgs longClickEventArgs)
        {
            var senderView = sender as View;
            var mediaPos = (int) senderView.GetTag(Resource.Id.Object_Position);
            var viewModel = Items[mediaPos];

            LongClickAction?.Invoke(viewModel);
        }

        private int CalculateCardColumns(BaseAniDroidActivity context, int columnCount)
        {
            if (columnCount > 0)
            {
                return columnCount;
            }

            var outMetrics = new DisplayMetrics();
            var density = context.Resources.DisplayMetrics.Density;

            context.WindowManager.DefaultDisplay.GetMetrics(outMetrics);
            var dpWidth = outMetrics.WidthPixels / density;

            return (int) dpWidth / 150;
        }

        public virtual RecyclerView.ViewHolder CreateCustomViewHolder(ViewGroup parent, int viewType)
        {
            return SetupCardItemViewHolder(new CardItem(Context.LayoutInflater.Inflate(Resource.Layout.View_CardItem_Horizontal, parent, false)));
        }

        public virtual void BindCustomViewHolder(RecyclerView.ViewHolder holder, int position)
        {
        }

        public virtual void BindCardViewHolder(CardItem holder, int position)
        {
        }

        public virtual CardItem SetupCardItemViewHolder(CardItem item)
        {
            return item;
        }
    }
}