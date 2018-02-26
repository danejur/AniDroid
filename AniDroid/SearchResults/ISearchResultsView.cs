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

namespace AniDroid.SearchResults
{
    public interface ISearchResultsView : IAniDroidView
    {
        void ShowMediaSearchResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable);
        void ShowCharacterSearchResults(IAsyncEnumerable<IPagedData<Character>> characterEnumerable);
        void ShowStaffSearchResults(IAsyncEnumerable<IPagedData<Staff>> staffEnumerable);
        void ShowUserSearchResults(IAsyncEnumerable<IPagedData<User>> userEnumerable);
        void ShowForumThreadSearchResults(IAsyncEnumerable<IPagedData<ForumThread>> forumThreadEnumerable);
        void ShowStudioSearchResults(IAsyncEnumerable<IPagedData<Studio>> studioEnumerable);
    }
}