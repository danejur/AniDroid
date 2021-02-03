using AniDroid.AniList.Models.ActivityModels;
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
        void SetupUserView(AniList.Models.UserModels.User user);
        void RefreshUserActivity();
        void UpdateActivity(int activityPosition, AniListActivity activity);
        void RemoveActivity(int activityPosition);
    }
}