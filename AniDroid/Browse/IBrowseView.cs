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

namespace AniDroid.Browse
{
    public interface IBrowseView : IAniDroidView
    {
        void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void UpdateMediaListItem(Media.MediaList mediaList);
        void RemoveMediaListItem(int mediaListId);
    }
}