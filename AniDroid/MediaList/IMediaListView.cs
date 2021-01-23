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
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;

namespace AniDroid.MediaList
{
    public interface IMediaListView : IAniDroidView
    {
        MediaType GetMediaType();
        void SetCollection(MediaListCollection collection);
        void UpdateMediaListItem(AniList.Models.MediaModels.MediaList mediaList);
        void ResetMediaListItem(int mediaId);
        void RemoveMediaListItem(int mediaListId);
        MediaListFilterModel GetMediaListFilter();
        void SetMediaListFilter(MediaListFilterModel filterModel);
    }
}