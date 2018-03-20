using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Models;

namespace AniDroid.AniListObject
{
    public interface IAniListActivityPresenter
    {
        Task ToggleLike(AniListActivity activity, int activityPosition);
        Task PostActivityReply(AniListActivity activity, int activityPosition, string activityText);
    }
}