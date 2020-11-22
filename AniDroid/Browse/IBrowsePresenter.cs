using System.Collections.Generic;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Models;

namespace AniDroid.Browse
{
    public interface IBrowsePresenter
    {
        void BrowseAniListMedia(BrowseMediaDto browseDto);
        BrowseMediaDto GetBrowseDto();
        IList<Media.MediaTag> GetMediaTags();
        IList<string> GetGenres();
    }
}