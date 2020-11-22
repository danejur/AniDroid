using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;

namespace AniDroid.Widgets
{
    public class DataRow : RelativeLayout
    {
        private ImageView _iconView;
        private LinearLayout _buttonView;
        private ImageView _buttonIconView;
        private TextView _textOneView;
        private TextView _textTwoView;

        protected DataRow(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DataRow(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public DataRow(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public DataRow(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public DataRow(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defStyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_DataRow, this, true);
            _iconView = FindViewById<ImageView>(Resource.Id.DataRow_Icon);
            _buttonView = FindViewById<LinearLayout>(Resource.Id.DataRow_Button);
            _buttonIconView = FindViewById<ImageView>(Resource.Id.DataRow_ButtonIcon);
            _textOneView = FindViewById<TextView>(Resource.Id.DataRow_TextOne);
            _textTwoView = FindViewById<TextView>(Resource.Id.DataRow_TextTwo);

            var typedVal = new TypedValue();
            context.Theme.ResolveAttribute(Resource.Attribute.Background_Text, typedVal, true);
            var defaultTextColor = typedVal.ResourceId;
            context.Theme.ResolveAttribute(Resource.Attribute.Primary_Dark, typedVal, true);
            var defaultButtonColor = typedVal.ResourceId;

            var attributes = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.DataRow, defStyleAttr ?? 0,
                defStyleRes ?? 0);

            int icon;
            int buttonIcon;
            string textOne;
            string textTwo;
            int textOneColor;
            int textTwoColor;
            int buttonColor;

            try
            {
                icon = attributes.GetResourceId(Resource.Styleable.DataRow_Icon,
                    Resource.Drawable.svg_star);
                buttonIcon = attributes.GetResourceId(Resource.Styleable.DataRow_ButtonIcon,
                    Resource.Drawable.svg_star);
                textOne = attributes.GetString(Resource.Styleable.DataRow_TextOne) ?? "";
                textTwo = attributes.GetString(Resource.Styleable.DataRow_TextTwo) ?? "";
                textOneColor = attributes.GetResourceId(Resource.Styleable.DataRow_TextOneColor,
                    defaultTextColor);
                textTwoColor = attributes.GetResourceId(Resource.Styleable.DataRow_TextTwoColor,
                    defaultTextColor);
                buttonColor = attributes.GetResourceId(Resource.Styleable.DataRow_ButtonColor, defaultButtonColor);

                ButtonClickable = attributes.GetBoolean(Resource.Styleable.DataRow_ButtonClickable, false);
                ButtonVisible = attributes.GetBoolean(Resource.Styleable.DataRow_ButtonVisible, false);
            }
            finally
            {
                attributes.Recycle();
            }

            SetIcon(icon);
            SetButtonIcon(buttonIcon);
            TextOne = textOne;
            TextTwo = textTwo;
            TextOneColor = ContextCompat.GetColor(context, textOneColor);
            TextTwoColor = ContextCompat.GetColor(context, textTwoColor);
            SetButtonColor(ContextCompat.GetColor(context, buttonColor));
            _buttonView.Click += (sender, args) => ButtonClick?.Invoke(sender, args);
        }

        public event EventHandler ButtonClick;

        public string TextOne
        {
            get => _textOneView?.Text;
            set
            {
                _textOneView.SetText(value, TextView.BufferType.Normal);
                _textOneView.Visibility = string.IsNullOrEmpty(value) ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public int TextOneColor
        {
            get => _textOneView.CurrentTextColor;
            set => _textOneView.SetTextColor(new Color(value));
        }

        public string TextTwo
        {
            get => _textTwoView?.Text;
            set
            {
                _textTwoView.SetText(value, TextView.BufferType.Normal);
                _textTwoView.Visibility = string.IsNullOrEmpty(value) ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public int TextTwoColor
        {
            get => _textTwoView.CurrentTextColor;
            set => _textTwoView.SetTextColor(new Color(value));
        }

        public void SetIcon(int resId)
        {
            _iconView?.SetImageResource(resId);
        }

        public void SetButtonColor(int color)
        {
            _buttonIconView.SetColorFilter(new Color(color), PorterDuff.Mode.SrcIn);
        }

        public void SetButtonIcon(int resId)
        {
            _buttonIconView?.SetImageResource(resId);
        }

        public ViewStates IconVisibility
        {
            get => _iconView.Visibility;
            set => _iconView.Visibility = value;
        }

        public bool ButtonVisible
        {
            get => _buttonView.Visibility == ViewStates.Visible;
            set => _buttonView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
        }

        public bool ButtonClickable
        {
            get => _buttonView.Clickable;
            set => _buttonView.Clickable = value;
        }
    }
}