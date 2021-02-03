using System.Collections.Generic;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.ForumModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.SearchResults
{
    public interface ISearchResultsView : IAniDroidView
    {
        void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowCharacterSearchResults(IAsyncEnumerable<OneOf<IPagedData<Character>, IAniListError>> characterEnumerable);
        void ShowStaffSearchResults(IAsyncEnumerable<OneOf<IPagedData<Staff>, IAniListError>> staffEnumerable);
        void ShowUserSearchResults(IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> userEnumerable);
        void ShowForumThreadSearchResults(IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> forumThreadEnumerable);
        void ShowStudioSearchResults(IAsyncEnumerable<OneOf<IPagedData<Studio>, IAniListError>> studioEnumerable);
        void UpdateMediaListItem(AniList.Models.MediaModels.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}