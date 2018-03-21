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
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Base;

namespace AniDroid.Widgets
{
    public class Picker : RelativeLayout
    {
        private IList<KeyValuePair<string, string>> _collection;
        private RecyclerView _recycler;
        private PickerRecyclerAdapter _adapter;
        private ImageView _plusButton;
        private ImageView _minusButton;

        public Picker(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public Picker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public Picker(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public Picker(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defSyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_Picker, this, true);
            _recycler = FindViewById<RecyclerView>(Resource.Id.Picker_Recycler);
            _plusButton = FindViewById<ImageView>(Resource.Id.Picker_PlusButton);
            _minusButton = FindViewById<ImageView>(Resource.Id.Picker_MinusButton);

            _plusButton.SetOnTouchListener(new LongTouchEventListener(IncrementCounter));
            _minusButton.SetOnTouchListener(new LongTouchEventListener(DecrementCounter));
        }

        public void SetItems(BaseAniDroidActivity context, IList<KeyValuePair<string, string>> items)
        {
            _collection = items;
            _recycler.SetAdapter(_adapter = new PickerRecyclerAdapter(context, items, 0));
        }

        private void IncrementCounter()
        {
            var pos = _adapter.GetSelectedPosition();

            if (pos + 1 != _adapter.ItemCount)
            {
                _adapter.SetSelectedPosition(pos + 1, true);
            }
        }

        private void DecrementCounter()
        {
            var pos = _adapter.GetSelectedPosition();

            if (pos > 0)
            {
                _adapter.SetSelectedPosition(pos - 1, true);
            }
        }

        private class PickerRecyclerAdapter : RecyclerView.Adapter
        {
            private readonly BaseAniDroidActivity _context;
            private readonly IList<KeyValuePair<string, string>> _items;
            private readonly PagerSnapHelper _snapHelper;
            private readonly PickerRecyclerLayoutManager _layoutManager;
            private readonly int _initialPosition;
            private RecyclerView _recyclerView;

            public override int ItemCount => _items.Count;

            public PickerRecyclerAdapter(BaseAniDroidActivity context, IList<KeyValuePair<string, string>> items,
                int initialPosition)
            {
                _context = context;
                _items = items;
                _initialPosition = initialPosition;
                _snapHelper = new PagerSnapHelper();

                var itemWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 40, _context.Resources.DisplayMetrics);
                var parentWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 100, _context.Resources.DisplayMetrics);
                _layoutManager = new PickerRecyclerLayoutManager(_context, parentWidth, itemWidth);
            }

            public int GetSelectedPosition()
            {
                return  _recyclerView.GetChildAdapterPosition(_snapHelper.FindSnapView(_layoutManager));
            }

            public void SetSelectedPosition(int position, bool smoothScroll = false)
            {
                if (smoothScroll)
                {
                    _recyclerView.SmoothScrollToPosition(position);
                }
                else
                {
                    _recyclerView.ScrollToPosition(position);
                }
            }

            public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
            {
                base.OnAttachedToRecyclerView(recyclerView);
                _recyclerView = recyclerView;
                recyclerView.SetLayoutManager(_layoutManager);
                _snapHelper.AttachToRecyclerView(recyclerView);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var item = _items[position];
                var viewHolder = holder as PickerViewHolder;

                viewHolder.ReadOnlyView.Text = item.Value;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                return new PickerViewHolder(_context.LayoutInflater.Inflate(Resource.Layout.Widget_PickerViewHolder,
                    parent, false));
            }

            private class PickerViewHolder : RecyclerView.ViewHolder
            {
                public TextView ReadOnlyView { get; }
                public EditText EditView { get; }

                public PickerViewHolder(View itemView) : base(itemView)
                {
                    ReadOnlyView = itemView.FindViewById<TextView>(Resource.Id.PickerViewHolder_ReadOnlyView);
                    EditView = itemView.FindViewById<EditText>(Resource.Id.PickerViewHolder_EditView);
                }
            }

            private class PickerRecyclerLayoutManager : LinearLayoutManager
            {
                private readonly int _parentWidth;
                private readonly int _itemWidth;

                public PickerRecyclerLayoutManager(Context context, int parentWidth, int itemWidth) : base(context, Horizontal, false)
                {
                    _parentWidth = parentWidth;
                    _itemWidth = itemWidth;
                }

                public override int PaddingLeft => (int) Math.Round(_parentWidth / 2m - _itemWidth / 2m);
                public override int PaddingRight => PaddingLeft;
            }
        }

        private class LongTouchEventListener : Java.Lang.Object, IOnTouchListener
        {
            private readonly Action _action;
            private Action _delayedAction;
            private Handler _handler;
            private bool _delayedActionStarted;

            public LongTouchEventListener(Action action)
            {
                _action = action;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        if (_handler != null)
                        {
                            return true;
                        }

                        _handler = new Handler();
                        _delayedActionStarted = false;

                        _delayedAction = () =>
                        {
                            _delayedActionStarted = true;
                            _action();
                            _handler?.PostDelayed(_delayedAction, 100);
                        };
                        _handler.PostDelayed(_delayedAction, 500);
                        return true;
                    case MotionEventActions.Up:
                        if (_handler == null)
                        {
                            return true;
                        }

                        if (!_delayedActionStarted)
                        {
                            _action();
                        }

                        _handler.RemoveCallbacks(_delayedAction);
                        _handler = null;
                        return true;
                }

                return false;
            }
        }
    }
}