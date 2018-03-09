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

namespace AniDroid.Discover
{
    public interface IDiscoverView : IAniDroidView
    {
        void ShowTrendingResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable);
        void ShowNewAnimeResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable);
        void ShowNewMangaResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable);
    }
}