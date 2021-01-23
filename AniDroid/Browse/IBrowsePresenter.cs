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
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;

namespace AniDroid.Browse
{
    public interface IBrowsePresenter
    {
        void BrowseAniListMedia(BrowseMediaDto browseDto);
        BrowseMediaDto GetBrowseDto();
        IList<MediaTag> GetMediaTags();
        IList<string> GetGenres();
    }
}