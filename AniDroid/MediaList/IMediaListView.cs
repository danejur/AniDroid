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
using AniDroid.AniListObject.Media;
using AniDroid.Base;

namespace AniDroid.MediaList
{
    public interface IMediaListView : IAniDroidView
    {
        Media.MediaType GetMediaType();
        void SetCollection(Media.MediaListCollection collection);
        void UpdateMediaListItem(Media.MediaList mediaList);
        void ResetMediaListItem(int mediaId);
    }
}