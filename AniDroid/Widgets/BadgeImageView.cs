using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AniDroid.Widgets
{
    public class BadgeImageView : AppCompatImageView
    {
        private const float SizeFactor = .3f;
        private const float TextSizeFactor = .5f;
        private const float HalfSizeFactor = SizeFactor / 2;

        private Paint _backgroundPaint;
        private Paint _textPaint;
        private string _text;
        private bool _enabled = true;

        public BadgeImageView(Context context) : base(context)
        {
            Init(context);
        }

        public BadgeImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public BadgeImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        public void Init(Context context)
        {
            _backgroundPaint = new Paint { Color = Color.Red, AntiAlias = true };
            _textPaint = new Paint
            {
                Color = Color.White,
                AntiAlias = true,
                TextAlign = Paint.Align.Center,
                TextSize = TextSizeFactor * Drawable.IntrinsicHeight
            };
            _textPaint.SetTypeface(Typeface.DefaultBold);
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (!_enabled || string.IsNullOrWhiteSpace(_text))
            {
                return;
            }

            var bounds = Drawable.Bounds;
            var x = (1 - HalfSizeFactor) * bounds.Width();
            var y = HalfSizeFactor * bounds.Height();
            canvas.DrawCircle(x, y, SizeFactor * bounds.Width(), _backgroundPaint);
            var textBounds = new Rect();
            _textPaint.GetTextBounds(_text, 0, _text.Length, textBounds);
            canvas.DrawText(_text, x, y + (float)textBounds.Height() / 2, _textPaint);
        }

        public bool IsEnabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (!_enabled)
                {
                    Drawable.InvalidateSelf();
                }
            }
        }

        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _enabled = false;
                Drawable.InvalidateSelf();
            }

            if (_text == text)
            {
                return;
            }

            _text = text;
            Drawable.InvalidateSelf();
        }
    }
}