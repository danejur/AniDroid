using System.Threading.Tasks;
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
        Task UpdateActivityAsync(AniListActivity activity, int activityPosition);
    }
}