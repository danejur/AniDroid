using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Base;

namespace AniDroid.Widgets
{
    public class DatePickerTextView : RelativeLayout
    {
        private EditText _editTextView;
        private ImageView _clearButton;

        private DateTime? _selectedDate;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => _editTextView.Text = (_selectedDate = value)?.ToShortDateString() ?? "";
        }

        public event EventHandler<DateChangedEventArgs> DateChanged;

        public DatePickerTextView(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public DatePickerTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public DatePickerTextView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public DatePickerTextView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defSyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_DatePickerTextView, this, true);
            _editTextView = FindViewById<EditText>(Resource.Id.DatePickerTextView_EditText);
            _clearButton = FindViewById<ImageView>(Resource.Id.DatePickerTextView_ClearButton);

            _editTextView.Click -= EditTextClick;
            _editTextView.Click += EditTextClick;

            _clearButton.Click -= ClearButtonClick;
            _clearButton.Click += ClearButtonClick;
        }

        private void EditTextClick(object sender, EventArgs eventArgs)
        {
            var defaultDate = _selectedDate ?? DateTime.Now;

            // TODO: fix theming

            DatePickerDialog dateDialog;

            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                var listener = new DatePickerTextViewOnDateSetListener(DateChanged, date => SelectedDate = date);
                dateDialog = new DatePickerDialog(Context, listener, defaultDate.Year, defaultDate.Month - 1, defaultDate.Day);

                listener.SetDialog(dateDialog);
            }
            else
            {
                dateDialog = new DatePickerDialog(Context);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    dateDialog.DatePicker.UpdateDate(defaultDate.Year, defaultDate.Month - 1, defaultDate.Day);
                    dateDialog.DatePicker.SetOnDateChangedListener(
                        new DatePickerTextViewOnDateChangedListener(dateDialog, DateChanged,
                            date => SelectedDate = date));
                }
                else
                {
                    dateDialog.DatePicker.Init(defaultDate.Year, defaultDate.Month - 1, defaultDate.Day,
                        new DatePickerTextViewOnDateChangedListener(dateDialog, DateChanged,
                            date => SelectedDate = date));
                }
            }

            dateDialog.Show();
        }

        private void ClearButtonClick(object sender, EventArgs eventArgs)
        {
            SelectedDate = null;
        }

        public class DateChangedEventArgs : EventArgs
        {
            public DateTime Date { get; }

            public DateChangedEventArgs(DateTime date)
            {
                Date = date;
            }
        }

        private class DatePickerTextViewOnDateChangedListener : Java.Lang.Object, DatePicker.IOnDateChangedListener
        {
            private readonly DatePickerDialog _dialog;
            private readonly EventHandler<DateChangedEventArgs> _dateChangedEventHandler;
            private readonly Action<DateTime> _dateSelectedAction;

            public DatePickerTextViewOnDateChangedListener(DatePickerDialog dialog, EventHandler<DateChangedEventArgs> dateChangedEventHandler, Action<DateTime> dateSelectedAction)
            {
                _dialog = dialog;
                _dateChangedEventHandler = dateChangedEventHandler;
                _dateSelectedAction = dateSelectedAction;
            }

            public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
            {
                var date = new DateTime(year, monthOfYear + 1, dayOfMonth);
                _dateSelectedAction?.Invoke(date);
                _dialog.Dismiss();
                _dateChangedEventHandler?.Invoke(_dialog.DatePicker, new DateChangedEventArgs(date));
            }
        }

        private class DatePickerTextViewOnDateSetListener : Java.Lang.Object, DatePickerDialog.IOnDateSetListener
        {
            private DatePickerDialog _dialog;
            private readonly EventHandler<DateChangedEventArgs> _dateChangedEventHandler;
            private readonly Action<DateTime> _dateSelectedAction;

            public DatePickerTextViewOnDateSetListener(EventHandler<DateChangedEventArgs> dateChangedEventHandler, Action<DateTime> dateSelectedAction)
            {
                _dateChangedEventHandler = dateChangedEventHandler;
                _dateSelectedAction = dateSelectedAction;
            }

            public void SetDialog(DatePickerDialog dialog)
            {
                _dialog = dialog;
            }

            public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
            {
                var date = new DateTime(year, month + 1, dayOfMonth);
                _dateSelectedAction?.Invoke(date);
                _dialog.Dismiss();
                _dateChangedEventHandler?.Invoke(_dialog.DatePicker, new DateChangedEventArgs(date));
            }
        }
    }
}