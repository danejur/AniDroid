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
using AniDroid.AniList.Interfaces;

namespace AniDroid.Base
{
    public interface IAniDroidView
    {
        void OnError(IAniListError error);
        void DisplaySnackbarMessage(string message, int length);
        void DisplayNotYetImplemented();
    }
}