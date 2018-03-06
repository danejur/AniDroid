using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AniDroid.Widgets
{
    public class AniDroidDataRow : RelativeLayout
    {
        private ImageView _iconView;
        private TextView _textOneView;
        private TextView _textTwoView;

        protected AniDroidDataRow(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AniDroidDataRow(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public AniDroidDataRow(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public AniDroidDataRow(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public AniDroidDataRow(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defStyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_AniDroidDataRow, this, true);
            _iconView = FindViewById<ImageView>(Resource.Id.AniDroidDataRow_Icon);
            _textOneView = FindViewById<TextView>(Resource.Id.AniDroidDataRow_TextOne);
            _textTwoView = FindViewById<TextView>(Resource.Id.AniDroidDataRow_TextTwo);

            var typedVal = new TypedValue();
            context.Theme.ResolveAttribute(Resource.Attribute.Background_Text, typedVal, true);
            var defaultTextColor = typedVal.ResourceId;

            var attributes = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.AniDroidDataRow, defStyleAttr ?? 0,
                defStyleRes ?? 0);

            int icon;
            string textOne;
            string textTwo;
            int textOneColor;
            int textTwoColor;

            try
            {
                icon = attributes.GetResourceId(Resource.Styleable.AniDroidDataRow_dataRowIcon,
                    Resource.Drawable.ic_star_white_24px);
                textOne = attributes.GetString(Resource.Styleable.AniDroidDataRow_dataRowTextOne) ?? "";
                textTwo = attributes.GetString(Resource.Styleable.AniDroidDataRow_dataRowTextTwo) ?? "";
                textOneColor = attributes.GetResourceId(Resource.Styleable.AniDroidDataRow_dataRowTextOneColor,
                    defaultTextColor);
                textTwoColor = attributes.GetResourceId(Resource.Styleable.AniDroidDataRow_dataRowTextTwoColor,
                    defaultTextColor);
            }
            finally
            {
                attributes.Recycle();
            }

            SetIcon(icon);
            TextOne = textOne;
            TextTwo = textTwo;
            TextOneColor = ContextCompat.GetColor(context, textOneColor);
            TextTwoColor = ContextCompat.GetColor(context, textTwoColor);
        }

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

        public ViewStates IconVisibility
        {
            get => _iconView.Visibility;
            set => _iconView.Visibility = value;
        }
    }
}