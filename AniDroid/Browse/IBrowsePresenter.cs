using System.Collections.Generic;
using AniDroid.AniList.Dto;
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