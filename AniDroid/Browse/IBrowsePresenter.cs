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

namespace AniDroid.Browse
{
    public interface IBrowsePresenter
    {
        void BrowseAniListMedia(BrowseMediaDto browseDto);
        BrowseMediaDto GetBrowseDto();
    }
}