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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Base;
using Java.Lang;

namespace AniDroid.Widgets
{
    public class Picker : RelativeLayout
    {
        private IList<string> _stringItems;
        private IList<int> _resIdItems;
        private bool _readOnly;
        private bool _drawablesShown;
        private ImageView _plusButton;
        private ImageView _minusButton;
        private TextView _readOnlyView;
        private EditText _editView;
        private ImageView _imageView;
        private int _selectedPosition;

        public int ItemCount => _drawablesShown ? _resIdItems?.Count ?? 0 : _stringItems?.Count ?? 0;

        public bool HasErrors { get; private set; }

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
            _readOnlyView = FindViewById<TextView>(Resource.Id.Picker_ReadOnlyView);
            _editView = FindViewById<EditText>(Resource.Id.Picker_EditView);
            _imageView = FindViewById<ImageView>(Resource.Id.Picker_ImageView);

            _stringItems = new List<string>();

            _plusButton.Click += IncrementCounter;
            _minusButton.Click += DecrementCounter;
        }

        public void SetItems(IList<string> items, bool readOnly, int defaultPosition)
        {
            _stringItems = items;
            _drawablesShown = false;
            _readOnly = readOnly;

            if (defaultPosition <= 0 || defaultPosition >= _stringItems.Count)
            {
                defaultPosition = 0;
            }

            _selectedPosition = defaultPosition;

            _editView.SetFilters(new IInputFilter[] {new ValidValuesInputFilter(_stringItems) });
            _editView.AfterTextChanged -= EditViewOnAfterTextChanged;
            _editView.AfterTextChanged += EditViewOnAfterTextChanged;

            DisplayValue();
        }

        public void SetDrawableItems(IList<int> resIds, int defaultPosition)
        {
            _resIdItems = resIds;
            _readOnly = true;
            _drawablesShown = true;

            if (defaultPosition <= 0 || defaultPosition >= _resIdItems.Count)
            {
                defaultPosition = 0;
            }

            _selectedPosition = defaultPosition;

            DisplayValue();
        }

        private void EditViewOnAfterTextChanged(object sender, AfterTextChangedEventArgs afterTextChangedEventArgs)
        {
            if (!string.IsNullOrEmpty(_editView.Text))
            {
                _selectedPosition = _stringItems.IndexOf(_editView.Text);
            }
        }

        public int GetSelectedPosition()
        {
            return _selectedPosition;
        }

        private void DisplayValue()
        {
            if (_selectedPosition == -1)
            {
                _readOnlyView.Visibility = ViewStates.Gone;
                _editView.Visibility = ViewStates.Gone;
                _imageView.Visibility = ViewStates.Gone;
                return;
            }

            if (_drawablesShown)
            {
                _imageView.SetImageResource(_resIdItems[_selectedPosition]);
                _readOnlyView.Visibility = ViewStates.Gone;
                _editView.Visibility = ViewStates.Gone;
                _imageView.Visibility = ViewStates.Visible;
            }
            else
            {
                _readOnlyView.Text = _editView.Text = _stringItems[_selectedPosition];
                _readOnlyView.Visibility = _readOnly ? ViewStates.Visible : ViewStates.Gone;
                _editView.Visibility = _readOnly ? ViewStates.Gone : ViewStates.Visible;
                _imageView.Visibility = ViewStates.Gone;
            }
        }

        private void IncrementCounter(object sender, EventArgs eventArgs)
        {
            if (_selectedPosition + 1 < ItemCount)
            {
                _selectedPosition += 1;
            }

            DisplayValue();
        }

        private void DecrementCounter(object sender, EventArgs eventArgs)
        {
            if (_selectedPosition > -1)
            {
                _selectedPosition -= 1;
            }

            DisplayValue();
        }

        private class ValidValuesInputFilter : Java.Lang.Object, IInputFilter
        {
            private readonly HashSet<string> _values;
            private readonly Java.Lang.String _emptyString = new Java.Lang.String();

            public ValidValuesInputFilter(IEnumerable<string> values)
            {
                _values = values.ToHashSet();
            }

            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                var arraySize = dest.Length();
                var retArr = new char[arraySize > dend + 1 ? arraySize : dend + 1];

                dest.ToArray().CopyTo(retArr, 0);

                var delOp = source.Length() == 0;
                for (var i = 0; i < retArr.Length; i++)
                {
                    if (dstart > i || dend < i)
                    {
                        continue;
                    }

                    if (delOp)
                    {
                        retArr[i] = '\0';
                    }
                    else
                    {
                        retArr[i] = source.CharAt(start);
                        if (start < end)
                        {
                            start += 1;
                        }
                    }
                }

                return _values.Contains(new string(retArr)) ? null : _emptyString;
            }
        }
    }
}