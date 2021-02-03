using System.Collections.Generic;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;
using OneOf;

namespace AniDroid.TorrentSearch
{
    public interface ITorrentSearchView : IAniDroidView
    {
        void ShowNyaaSiSearchResults(
            IAsyncEnumerable<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> searchEnumerable);
    }
}