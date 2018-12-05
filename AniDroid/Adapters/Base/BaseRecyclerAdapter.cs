using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Base;

namespace AniDroid.Adapters.Base
{
    public abstract class BaseRecyclerAdapter<T> : BaseRecyclerAdapter
    {
        private int _orientation = LinearLayoutManager.Vertical;
        private RecyclerView.ItemDecoration _decoration;
        protected ColorStateList DefaultIconColor;
        protected ColorStateList FavoriteIconColor;
        protected bool CustomCardUseItemDecoration;
        protected int CardColumnCount;
        public RecyclerCardType CardType { get; protected set; }
        public List<T> Items { get; protected set; }
        public sealed override int ItemCount => Items.Count;

        protected BaseRecyclerAdapter(BaseAniDroidActivity context, List<T> items, RecyclerCardType cardType) : base(context)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            CardType = cardType;
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
                BindCardViewHolder(holder as CardItem, position);
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
                    recyclerView.SetLayoutManager(new GridLayoutManager(Context, CalculateCardColumns(Context), _orientation, false));
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

        private int CalculateCardColumns(Activity context)
        {
            if (CardColumnCount > 0)
            {
                return CardColumnCount;
            }

            var outMetrics = new DisplayMetrics();
            var density = context.Resources.DisplayMetrics.Density;

            context.WindowManager.DefaultDisplay.GetMetrics(outMetrics);
            var dpWidth = outMetrics.WidthPixels / density;

            return (int)dpWidth / 150;
        }
    }

    public abstract class BaseRecyclerAdapter : RecyclerView.Adapter
    {
        protected RecyclerView RecyclerView { get; private set; }
        protected BaseAniDroidActivity Context { get; private set; }

        protected BaseRecyclerAdapter(BaseAniDroidActivity context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            RecyclerView = recyclerView;
        }

        public void SetContext(BaseAniDroidActivity context)
        {
            Context = context;
        }

        public enum RecyclerCardType
        {
            Custom = -1,
            Vertical = 0,
            Horizontal = 1,
            FlatHorizontal = 2
        }

        public class CardItem : RecyclerView.ViewHolder
        {
            public LinearLayout Container { get; set; }
            public ImageView Image { get; set; }
            public TextView Name { get; set; }
            public TextView DetailPrimary { get; set; }
            public TextView DetailSecondary { get; set; }
            public CardView ContainerCard { get; set; }
            public View Button { get; set; }
            public ImageView ButtonIcon { get; set; }

            public ColorStateList DefaultNameColor { get; }
            public ColorStateList DefaultBackgroundColor { get; }

            public CardItem(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<LinearLayout>(Resource.Id.CardItem_Container);
                Image = itemView.FindViewById<ImageView>(Resource.Id.CardItem_Image);
                Name = itemView.FindViewById<TextView>(Resource.Id.CardItem_Name);
                DetailPrimary = itemView.FindViewById<TextView>(Resource.Id.CardItem_DetailPrimary);
                DetailSecondary = itemView.FindViewById<TextView>(Resource.Id.CardItem_DetailSecondary);
                ContainerCard = itemView.FindViewById<CardView>(Resource.Id.CardItem_Card);
                Button = itemView.FindViewById(Resource.Id.CardItem_Button);
                ButtonIcon = itemView.FindViewById<ImageView>(Resource.Id.CardItem_ButtonIcon);

                DefaultNameColor = itemView.FindViewById<TextView>(Resource.Id.CardItem_Name).TextColors;
                DefaultBackgroundColor = itemView.FindViewById<CardView>(Resource.Id.CardItem_Card).CardBackgroundColor;
            }
        }

        public class DefaultItemDecoration : RecyclerView.ItemDecoration
        {
            private readonly Drawable _divider;

            public DefaultItemDecoration(Context context)
            {
                var typedValue = new TypedValue();
                context.Theme.ResolveAttribute(Resource.Attribute.ListItem_Divider, typedValue, true);
                _divider = new ColorDrawable(new Color(ContextCompat.GetColor(context, typedValue.ResourceId)));
            }

            public override void OnDrawOver(Canvas cValue, RecyclerView parent, RecyclerView.State state)
            {
                var left = parent.PaddingLeft;
                var right = parent.Width - parent.PaddingRight;

                var childCount = parent.ChildCount;
                for (var i = 0; i < childCount; i++)
                {
                    var child = parent.GetChildAt(i);

                    var layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;

                    var top = child.Bottom + layoutParams.BottomMargin + (int)child.TranslationY;
                    var bottom = top + 2;

                    _divider.Alpha = (int)(child.Alpha * 255);
                    _divider.SetBounds(left, top, right, bottom);
                    _divider.Draw(cValue);
                }
            }
        }

        public class StableIdItem<T> : IStableIdItem
        {
            public StableIdItem(T item)
            {
                Item = item;
                StableId = (long)new Random().NextDouble();
            }

            public T Item { get; set; }
            public long StableId { get; }
        }

        public interface IStableIdItem
        {
            long StableId { get; }
        }
    }
}