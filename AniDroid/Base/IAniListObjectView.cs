using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AniDroid.Base
{
    public interface IAniListObjectView : IAniDroidView
    {
        void Share();
        void SetLoadingShown();
        void SetContentShown();
        void SetErrorShown(string title, string message);
        void SetIsFavorite(bool isFavorite);
        void SetShareText(string title, string uri);
        void SetupToolbar(string text);
        void SetStandaloneActivity();
    }
}