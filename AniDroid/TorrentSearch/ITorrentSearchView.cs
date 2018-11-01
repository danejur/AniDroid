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