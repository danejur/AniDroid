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
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.AniListObject.User
{
    public interface IUserView : IAniListObjectView
    {
        int? GetUserId();
        string GetUserName();
        void SetIsFollowing(bool isFollowing, bool showNotification);
        void SetCanFollow();
        void SetCanMessage();
        void SetupUserView(AniList.Models.User user);
        void RefreshUserActivity();
        void UpdateActivity(int activityPosition, AniListActivity activity);
    }
}