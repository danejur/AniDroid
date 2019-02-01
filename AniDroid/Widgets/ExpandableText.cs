using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AniDroid.Widgets
{
    public class ExpandableText : RelativeLayout
    {
        private bool _textProcessed;
        private TextView _text;
        private TextView _button;

        public Action<TextView> ExpandTextAction { get; set; }

        public ExpandableText(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public ExpandableText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public ExpandableText(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public ExpandableText(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defStyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_ExpandableText, this, true);
            _text = FindViewById<TextView>(Resource.Id.ExpandableText_Text);
            _button = FindViewById<TextView>(Resource.Id.ExpandableText_ExpandButton);

            var typedVal = new TypedValue();
            context.Theme.ResolveAttribute(Resource.Attribute.Background_Text, typedVal, true);
            var defaultTextColor = typedVal.ResourceId;

            var attributes = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ExpandableText,
                defStyleAttr ?? 0,
                defStyleRes ?? 0);

            string text;
            int textColor;
            int buttonColor;
            float textSize;

            try
            {
                text = attributes.GetString(Resource.Styleable.ExpandableText_Text) ?? "";
                textColor = attributes.GetResourceId(Resource.Styleable.ExpandableText_TextColor,
                    defaultTextColor);
                buttonColor = attributes.GetResourceId(Resource.Styleable.ExpandableText_ButtonColor,
                    defaultTextColor);
                textSize = attributes.GetDimension(Resource.Styleable.ExpandableText_TextSize,
                    Resources.DisplayMetrics.Density * 14);
            }
            finally
            {
                attributes.Recycle();
            }
        }

        private void CalculateTextLength(object sender, ViewTreeObserver.PreDrawEventArgs e)
        {
            if (_textProcessed)
            {
                return;
            }

            if (_text.LineCount > 3)
            {
                _text.SetMaxLines(3);
                _text.Ellipsize = TextUtils.TruncateAt.End;
                _button.Visibility = ViewStates.Visible;
                _button.Clickable = true;
                _button.Click -= ExpandText;
                _button.Click += ExpandText;
            }
            else
            {
                _button.Visibility = ViewStates.Gone;
            }

            _textProcessed = true;
        }

        private void ExpandText(object sender, EventArgs e)
        {
            _text.SetMaxLines(int.MaxValue);
            _button.Visibility = ViewStates.Gone;
            ExpandTextAction?.Invoke(_text);
        }

        public string Text
        {
            get => _text.Text;
            set
            {
                _textProcessed = false;
                _text.Text = value;
                _text.ViewTreeObserver.PreDraw -= CalculateTextLength;
                _text.ViewTreeObserver.PreDraw += CalculateTextLength;
            }
        }

        public ICharSequence TextFormatted
        {
            get => _text.TextFormatted;
            set
            {
                _textProcessed = false;
                _text.TextFormatted = value;
                _text.ViewTreeObserver.PreDraw -= CalculateTextLength;
                _text.ViewTreeObserver.PreDraw += CalculateTextLength;
            }
        }

        public float TextSize
        {
            get => _text.TextSize;
            set => _text.TextSize = value;
        }

        public int TextColor
        {
            get => _text.CurrentTextColor;
            set => _text.SetTextColor(new Color(value));
        }

        public int ButtonColor
        {
            get => _button.CurrentTextColor;
            set => _button.SetTextColor(new Color(value));
        }
    }
}