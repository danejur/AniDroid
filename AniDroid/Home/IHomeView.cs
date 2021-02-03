using System.Collections.Generic;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Home
{
    public interface IHomeView : IAniDroidView
    {
        void ShowUserActivity(IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> activityEnumerable, int userId);
        void ShowAllActivity(IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> activityEnumerable, int userId);
        void UpdateActivity(int activityPosition, AniListActivity activity);
        void RemoveActivity(int activityPosition);
        void RefreshActivity();
    }
}