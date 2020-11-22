using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.MediaList
{
    public interface IMediaListView : IAniDroidView
    {
        Media.MediaType GetMediaType();
        void SetCollection(Media.MediaListCollection collection);
        void UpdateMediaListItem(Media.MediaList mediaList);
        void ResetMediaListItem(int mediaId);
        void RemoveMediaListItem(int mediaListId);
        MediaListFilterModel GetMediaListFilter();
        void SetMediaListFilter(MediaListFilterModel filterModel);
    }
}