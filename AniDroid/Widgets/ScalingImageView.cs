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

namespace AniDroid.Widgets
{
    public class ScalingImageView : ImageView
    {
        private float _heightToWidthRatio;
        private float _widthToHeightRatio;

        public ScalingImageView(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public ScalingImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public ScalingImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public ScalingImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defStyleAttr, int? defStyleRes)
        {
            var attributes = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ScalingImageView, defStyleAttr ?? 0,
                defStyleRes ?? 0);
            _heightToWidthRatio = attributes.GetFloat(Resource.Styleable.ScalingImageView_HeightToWidthRatio, 0);
            _widthToHeightRatio = attributes.GetFloat(Resource.Styleable.ScalingImageView_WidthToHeightRatio, 0);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            if (_heightToWidthRatio > 0)
            {
                SetMeasuredDimension(MeasuredWidth, (int) (MeasuredWidth * _heightToWidthRatio));
            }
            else if (_widthToHeightRatio > 0)
            {
                SetMeasuredDimension((int) (MeasuredHeight * _widthToHeightRatio), MeasuredHeight);
            }
        }
    }
}