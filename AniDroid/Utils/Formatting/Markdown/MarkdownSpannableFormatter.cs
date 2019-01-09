using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Square.PicassoLib;

namespace AniDroid.Utils.Formatting.Markdown
{
    public static class MarkdownSpannableFormatter
    {
        private const int PlaceholderImage = Resource.Drawable.svg_image;
        private const string YoutubeThumbnailUrl = "https://img.youtube.com/vi/{0}/hqdefault.jpg";
        private const string YoutubeLinkUrl = "https://www.youtube.com/watch?v={0}";
        private const int MaxImageWidth = 250;

        private static readonly Regex ImageRegex = new Regex(@"img(\d*)\(([^\)]*)\)", RegexOptions.Compiled);
        private static readonly Regex YoutubeRegex = new Regex(@"youtube\(([^\)]*)\)", RegexOptions.Compiled);

        public static void FormatMarkdownSpannable(Context context, ISpannable spannable)
        {
            FormatImages(context, spannable);
            FormatYoutube(context, spannable);
        }

        private static void FormatImages(Context context, ISpannable spannable)
        {
            var text = spannable?.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var matches = ImageRegex.Matches(text);

            foreach (var match in matches.ToList())
            {
                var drawable = ContextCompat.GetDrawable(Application.Context, PlaceholderImage);
                drawable.SetBounds(0, 0, drawable.IntrinsicWidth, drawable.IntrinsicHeight);

                var imageSpan = new ImageSpan(drawable, SpanAlign.Baseline);

                spannable.SetSpan(imageSpan, match.Index, match.Index + match.Length,
                    SpanTypes.InclusiveExclusive);

                if (!int.TryParse(match.Groups[1].Value, out var width))
                {
                    width = 250;
                }

                width = Math.Min(width, MaxImageWidth);

                Picasso.Get().Load(match.Groups[2].Value)
                    .Into(new DrawableTarget(spannable, match.Index, match.Index + match.Length, (int)(width * context.Resources.DisplayMetrics.Density)));
            }
        }

        private static void FormatYoutube(Context context, ISpannable spannable)
        {
            var text = spannable?.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var matches = YoutubeRegex.Matches(text);

            var playIcon = ContextCompat.GetDrawable(context, Resource.Drawable.svg_play);
            playIcon.SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
            playIcon.SetAlpha(150);

            foreach (var match in matches.ToList())
            {
                var drawable = ContextCompat.GetDrawable(Application.Context, PlaceholderImage);
                drawable.SetBounds(0, 0, drawable.IntrinsicWidth, drawable.IntrinsicHeight);

                var imageSpan = new ImageSpan(drawable, SpanAlign.Baseline);

                var clickSpan = new CustomClickableSpan();
                clickSpan.Click += (sender, e) =>
                {
                    var intent = new Intent(Intent.ActionView,
                        Android.Net.Uri.Parse(string.Format(YoutubeLinkUrl, match.Groups[1].Value)));
                    context.StartActivity(intent);
                };

                spannable.SetSpan(clickSpan, match.Index, match.Index + match.Length,
                    SpanTypes.InclusiveExclusive);

                spannable.SetSpan(imageSpan, match.Index, match.Index + match.Length,
                    SpanTypes.InclusiveExclusive);

                Picasso.Get().Load(string.Format(YoutubeThumbnailUrl, match.Groups[1].Value))
                    .Into(new DrawableTarget(spannable, match.Index, match.Index + match.Length,
                        (int) (250 * context.Resources.DisplayMetrics.Density), playIcon));
            }
        }

        private class DrawableTarget : Java.Lang.Object, ITarget
        {
            private readonly ISpannable _spannable;
            private readonly int _start;
            private readonly int _end;
            private readonly int _width;
            private readonly Drawable _playIcon;

            public DrawableTarget(ISpannable spannable, int start, int end, int width, Drawable playIcon = null)
            {
                _spannable = spannable;
                _start = start;
                _end = end;
                _width = width;
                _playIcon = playIcon;
            }

            public void OnBitmapFailed(Java.Lang.Exception e, Drawable errorDrawable)
            {
            }

            public void OnBitmapLoaded(Bitmap bitmap, Picasso.LoadedFrom @from)
            {
                var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, _width, (int)(bitmap.Height / ((float)bitmap.Width / _width)), true);

                if (_playIcon != null)
                {
                    _playIcon.SetBounds((int) (scaledBitmap.Width * .35), (int) (scaledBitmap.Height * .3),
                        (int) (scaledBitmap.Width * .65), (int) (scaledBitmap.Height * .7));
                    _playIcon.Draw(new Canvas(scaledBitmap));
                }

                var drawable = new BitmapDrawable(Application.Context.Resources, scaledBitmap);
                drawable.SetBounds(0, 0, drawable.IntrinsicWidth, drawable.IntrinsicHeight);

                var imageSpan = new ImageSpan(drawable, SpanAlign.Baseline);
                _spannable.SetSpan(imageSpan, _start, _end,
                    SpanTypes.InclusiveExclusive);
            }

            public void OnPrepareLoad(Drawable placeHolderDrawable)
            {
            }
        }

        private class CustomClickableSpan : ClickableSpan
        {
            public event EventHandler Click;

            public override void OnClick(View widget)
            {
                Click?.Invoke(widget, new EventArgs());
            }
        }
    }
}