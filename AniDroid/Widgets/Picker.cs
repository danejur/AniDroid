using System;
using System.Collections.Generic;
using System.Globalization;
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
using Math = System.Math;
using Single = System.Single;

namespace AniDroid.Widgets
{
    public class Picker : RelativeLayout
    {
        private PickerType _type;
        private ImageView _plusButton;
        private ImageView _minusButton;
        private TextView _readOnlyView;
        private EditText _editView;
        private ImageView _imageView;
        private IList<int> _resIdItems;
        private IList<string> _stringItems;
        private float _maxValue;
        private uint _precision;
        private float? _currentValue;

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

            _plusButton.Click += IncrementCounter;
            _minusButton.Click += DecrementCounter;
        }

        public void SetDrawableItems(IList<int> resIds, int? defaultPosition)
        {
            _resIdItems = resIds;
            _type = PickerType.Drawable;

            _editView.Visibility = _readOnlyView.Visibility = ViewStates.Gone;

            if (defaultPosition < 0 || defaultPosition >= _resIdItems.Count)
            {
                defaultPosition = null;
            }

            _currentValue = defaultPosition;

            DisplayValue();
        }

        public void SetStringItems(IList<string> strings, int? defaultPosition)
        {
            _stringItems = strings;
            _type = PickerType.Strings;

            _readOnlyView.Visibility = ViewStates.Visible;
            _editView.Visibility = _imageView.Visibility = ViewStates.Gone;

            if (defaultPosition < 0 || defaultPosition >= _stringItems.Count)
            {
                defaultPosition = null;
            }

            _currentValue = defaultPosition;

            DisplayValue();
        }

        public void SetMaxValue(float max, uint precision, bool readOnly, float? defaultValue)
        {
            _maxValue = max;
            _precision = precision;
            _type = readOnly ? PickerType.ReadOnly : PickerType.Editable;

            _imageView.Visibility = ViewStates.Gone;
            _editView.Visibility = readOnly ? ViewStates.Gone : ViewStates.Visible;
            _readOnlyView.Visibility = readOnly ? ViewStates.Visible : ViewStates.Gone;

            if (_precision == 0)
            {
                _editView.InputType = InputTypes.ClassNumber;
            }

            _editView.AfterTextChanged += (sender, args) =>
            {
                var str = args.Editable.ToString();
                var length = str.Length;

                if (!float.TryParse(str, out var parsedResult) && args.Editable.All(x => x == '.') ||
                    parsedResult < 0)
                {
                    _currentValue = 0;
                }
                else if (parsedResult > _maxValue)
                {
                    _currentValue = _maxValue;
                    args.Editable.Replace(0, length, _maxValue.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    _currentValue = parsedResult;
                }

            };

           

            _currentValue = defaultValue;

            DisplayValue();
        }

        public float? GetValue() => _currentValue;

        public void SetValue(float? value)
        {
            _currentValue = value;

            DisplayValue();
        }

        private int? GetCollectionPosition(float? value)
        {
            var count = _type == PickerType.Drawable ? _resIdItems?.Count : _stringItems?.Count;
            return (int?) (value < 0 || value >= count ? null : value);
        }

        private void DisplayValue()
        {
            if (_type == PickerType.Drawable)
            {
                var position = GetCollectionPosition(_currentValue);

                if (position.HasValue)
                {
                    _imageView.Visibility = ViewStates.Visible;
                    _imageView.SetImageResource(_resIdItems[position.Value]);
                }
                else
                {
                    _imageView.Visibility = ViewStates.Gone;
                }

                return;
            }
            if (_type == PickerType.Strings)
            {
                var position = GetCollectionPosition(_currentValue);

                if (position.HasValue)
                {
                    _readOnlyView.Visibility = ViewStates.Visible;
                    _readOnlyView.Text = _stringItems[position.Value];
                }
                else
                {
                    _readOnlyView.Visibility = ViewStates.Gone;
                }

                return;
            }

            _editView.Text = _readOnlyView.Text = _currentValue?.ToString("N" + _precision) ?? "";
        }

        private void IncrementCounter(object sender, EventArgs eventArgs)
        {
            var alteredVal = (_currentValue ?? (_type == PickerType.Drawable || _type == PickerType.Strings ? -1 : 0)) + 1 / (float)Math.Pow(10, _precision);

            if ((_type == PickerType.Drawable || _type == PickerType.Strings) && !GetCollectionPosition(alteredVal).HasValue || (_type != PickerType.Drawable && _type != PickerType.Strings) && alteredVal > _maxValue)
            {
                return;
            }

            _currentValue = alteredVal;

            DisplayValue();
        }

        private void DecrementCounter(object sender, EventArgs eventArgs)
        {
            var alteredVal = _currentValue - 1;

            if (alteredVal < 0)
            {
                alteredVal = null;
            }

            _currentValue = alteredVal;

            DisplayValue();
        }

        private enum PickerType
        {
            ReadOnly = 0,
            Editable = 1,
            Drawable = 2,
            Strings = 3
        }
    }
}