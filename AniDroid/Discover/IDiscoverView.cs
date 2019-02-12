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

namespace AniDroid.Discover
{
    public interface IDiscoverView : IAniDroidView
    {
        void ShowCurrentlyAiringResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowTrendingAnimeResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowTrendingMangaResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowNewAnimeResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowNewMangaResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void UpdateMediaListItem(Media.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}