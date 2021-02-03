using System.Collections.Generic;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Discover
{
    public interface IDiscoverView : IAniDroidView
    {
        void ShowCurrentlyAiringResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowTrendingAnimeResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowTrendingMangaResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowNewAnimeResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowNewMangaResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void UpdateMediaListItem(AniList.Models.MediaModels.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}