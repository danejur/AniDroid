using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.Graphics.Drawable;

namespace AniDroid.Widgets
{
    public class BadgeDrawerArrowDrawable : DrawerArrowDrawable
    {
        private const float SizeFactor = .3f;
        private const float TextSizeFactor = .5f;
        private const float HalfSizeFactor = SizeFactor / 2;

        private readonly Paint _backgroundPaint;
        private readonly Paint _textPaint;
        private string _text;
        private bool _enabled = true;

        public BadgeDrawerArrowDrawable(Context context) : base(context)
        {
            _backgroundPaint = new Paint {Color = Android.Graphics.Color.Red, AntiAlias = true};
            _textPaint = new Paint
            {
                Color = Android.Graphics.Color.White,
                AntiAlias = true,
                TextAlign = Paint.Align.Center,
                TextSize = TextSizeFactor * IntrinsicHeight
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

            var bounds = Bounds;
            var x = (1 - HalfSizeFactor) * bounds.Width();
            var y = HalfSizeFactor * bounds.Height();
            canvas.DrawCircle(x, y, SizeFactor * bounds.Width(), _backgroundPaint);

            var textBounds = new Rect();
            _textPaint.GetTextBounds(_text, 0, _text.Length, textBounds);
            canvas.DrawText(_text, x, y + (float) textBounds.Height() / 2, _textPaint);
        }

        public bool IsEnabled
        {
            get => _enabled;
            set { _enabled = value;
                if (!_enabled)
                {
                    InvalidateSelf();
                }
            }
        }

        public string Text {
            get => _text;
            set
            {
                if (_text == value)
                {
                    return;
                }

                _text = value;
                InvalidateSelf();
            }
        }
    }
}