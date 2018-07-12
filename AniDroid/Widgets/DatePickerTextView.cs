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
            // TODO: fix themeing
            var dateDialog = new DatePickerDialog(Context);

            dateDialog.UpdateDate(_selectedDate ?? DateTime.Now);

            dateDialog.DatePicker.DateChanged += (dateSender, dateE) =>
            {
                var date = new DateTime(dateE.Year, dateE.MonthOfYear + 1, dateE.DayOfMonth);
                SelectedDate = date;
                dateDialog.Dismiss();
                DateChanged?.Invoke(dateDialog.DatePicker, new DateChangedEventArgs(date));
            };

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
    }
}