using System.Collections.Generic;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Browse
{
    public interface IBrowseView : IAniDroidView
    {
        void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void UpdateMediaListItem(Media.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}