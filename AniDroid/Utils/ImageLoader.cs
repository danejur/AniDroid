using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Square.Picasso;

namespace AniDroid.Utils
{
    public static class ImageLoader
    {
        public static void LoadImage(ImageView imageView, string url, bool showLoading = true)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var req = Picasso.Get().Load(url);

            if (showLoading)
            {
                req = req.Placeholder(Resource.Drawable.svg_image);
            }

            req.Into(imageView);
        }
    }
}