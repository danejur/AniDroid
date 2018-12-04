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
        void UpdateMediaListItem(Media.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}