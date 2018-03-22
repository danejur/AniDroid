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
using Single = System.Single;

namespace AniDroid.Widgets
{
    public class Picker : RelativeLayout
    {
        private IList<string> _stringItems;
        private IList<int> _resIdItems;
        private Tuple<float, float> _minMaxDecimalValue;
        private PickerType _type;
        private ImageView _plusButton;
        private ImageView _minusButton;
        private TextView _readOnlyView;
        private EditText _editView;
        private EditText _editDecimalView;
        private ImageView _imageView;
        private int _selectedPosition;

        public int ItemCount => _type == PickerType.Drawable ? _resIdItems?.Count ?? 0 : _stringItems?.Count ?? 0;

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
            _editDecimalView = FindViewById<EditText>(Resource.Id.Picker_EditDecimalView);

            _stringItems = new List<string>();

            _plusButton.Click += IncrementCounter;
            _minusButton.Click += DecrementCounter;
        }

        public void SetItems(IList<string> items, bool readOnly, int defaultPosition = -1)
        {
            _stringItems = items;
            _type = readOnly ? PickerType.ReadOnly : PickerType.Editable;

            if (defaultPosition <= -1 || defaultPosition >= _stringItems.Count)
            {
                defaultPosition = -1;
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
            _type = PickerType.Drawable;

            if (defaultPosition <= 0 || defaultPosition >= _resIdItems.Count)
            {
                defaultPosition = 0;
            }

            _selectedPosition = defaultPosition;

            DisplayValue();
        }

        public void SetDecimalMinMax(float min, float max, float defaultValue)
        {
            _minMaxDecimalValue = new Tuple<float, float>(min, max);
            _editDecimalView.SetFilters(new IInputFilter[] {new MinMaxInputFilter(min, max)});
            _type = PickerType.Decimal;
            _editDecimalView.Text = defaultValue.ToString(CultureInfo.InvariantCulture);

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

        public float? GetDecimalValue()
        {
            return float.TryParse(_editDecimalView.Text, out var outVal) ? outVal : (float?)null;
        }

        private void DisplayValue()
        {
            if (_selectedPosition == -1)
            {
                _readOnlyView.Visibility = ViewStates.Gone;
                _editView.Visibility = ViewStates.Gone;
                _imageView.Visibility = ViewStates.Gone;
                _editDecimalView.Visibility = ViewStates.Gone;
                return;
            }

            switch (_type)
            {
                case PickerType.Drawable:
                    _imageView.SetImageResource(_resIdItems[_selectedPosition]);
                    _readOnlyView.Visibility = ViewStates.Gone;
                    _editView.Visibility = ViewStates.Gone;
                    _imageView.Visibility = ViewStates.Visible;
                    _editDecimalView.Visibility = ViewStates.Gone;
                    break;
                case PickerType.Editable:
                    _readOnlyView.Text = _editView.Text = _stringItems[_selectedPosition];
                    _readOnlyView.Visibility = ViewStates.Gone;
                    _editView.Visibility = ViewStates.Visible;
                    _imageView.Visibility = ViewStates.Gone;
                    _editDecimalView.Visibility = ViewStates.Gone;
                    break;
                case PickerType.ReadOnly:
                    _readOnlyView.Text = _editView.Text = _stringItems[_selectedPosition];
                    _readOnlyView.Visibility = ViewStates.Visible;
                    _editView.Visibility = ViewStates.Gone;
                    _imageView.Visibility = ViewStates.Gone;
                    _editDecimalView.Visibility = ViewStates.Gone;
                    break;
                case PickerType.Decimal:
                    _readOnlyView.Visibility = ViewStates.Gone;
                    _editView.Visibility = ViewStates.Gone;
                    _imageView.Visibility = ViewStates.Gone;
                    _editDecimalView.Visibility = ViewStates.Visible;
                    break;

            }
        }

        private void IncrementCounter(object sender, EventArgs eventArgs)
        {
            if (_type == PickerType.Decimal && float.TryParse(_editDecimalView.Text, out var parsedVal))
            {
                parsedVal += .1f;
                _editDecimalView.Text = parsedVal.ToString("#.#");
            }
            else if (_selectedPosition + 1 < ItemCount)
            {
                _selectedPosition += 1;
            }

            DisplayValue();
        }

        private void DecrementCounter(object sender, EventArgs eventArgs)
        {
            if (_type == PickerType.Decimal && float.TryParse(_editDecimalView.Text, out var parsedVal))
            {
                parsedVal -= .1f;
                _editDecimalView.Text = parsedVal.ToString("#.#");
            }
            else if (_selectedPosition > -1)
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
                    else if (start < end)
                    {
                        retArr[i] = source.CharAt(start);
                        start += 1;
                    }
                    else
                    {
                        retArr[i] = '\0';
                    }
                }

                return _values.Contains(new string(retArr)) ? null : _emptyString;
            }   
        }

        private class MinMaxInputFilter : Java.Lang.Object, IInputFilter
        {
            private readonly float _min;
            private readonly float _max;
            private readonly Java.Lang.String _emptyString = new Java.Lang.String();

            public MinMaxInputFilter(float min, float max)
            {
                _min = min;
                _max = max;
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
                    else if (start < end)
                    {
                        retArr[i] = source.CharAt(start);
                        start += 1;
                    }
                    else
                    {
                        retArr[i] = '\0';
                    }
                }

                if (!float.TryParse(new string(retArr), out var parsedResult))
                {
                    return retArr.Length > 0 && retArr.All(x => x == '.' || x == '-') ? null : _emptyString;
                }

                return parsedResult >= _min && parsedResult <= _max ? null : _emptyString;
            }
        }

        private enum PickerType
        {
            ReadOnly = 0,
            Editable = 1,
            Decimal = 2,
            Drawable = 3
        }
    }
}