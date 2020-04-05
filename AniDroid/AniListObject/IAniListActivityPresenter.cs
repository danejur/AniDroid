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
        Task ToggleActivityLikeAsync(AniListActivity activity, int activityPosition);
        Task PostActivityReplyAsync(AniListActivity activity, int activityPosition, string activityText);
        Task EditStatusActivityAsync(AniListActivity activity, int activityPosition, string updateText);
        Task DeleteActivityAsync(int activityId, int activityPosition);
        Task ToggleActivityReplyLikeAsync(AniListActivity.ActivityReply activityReply, int activityPosition);
        Task EditActivityReplyAsync(AniListActivity.ActivityReply activityReply, int activityPosition,
            string updateText);
        Task<bool> DeleteActivityReplyAsync(AniListActivity.ActivityReply activityReply, int activityPosition);

    }
}