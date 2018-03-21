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
            _plusButton = FindViewById<ImageView>(Resource.Id.Picker_PlusButton);
            _minusButton = FindViewById<ImageView>(Resource.Id.Picker_MinusButton);

            _plusButton.Click += IncrementCounter;
            _minusButton.Click += DecrementCounter;
        }

        public void SetItems(BaseAniDroidActivity context, IList<KeyValuePair<string, string>> items)
        {
            _collection = items;
        }

        private void IncrementCounter(object sender, EventArgs eventArgs)
        {
            
        }

        private void DecrementCounter(object sender, EventArgs eventArgs)
        {
            
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