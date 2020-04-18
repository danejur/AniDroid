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
using AniDroid.AniList.Models;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Main
{
    public interface IMainView : IAniDroidView
    {
        int GetVersionCode();
        void DisplayWhatsNewDialog();
        void SetAuthenticatedNavigationVisibility(bool isAuthenticated);
        void OnMainViewSetup();
        void SetNotificationCount(int count);
        void LogoutUser();
    }
}