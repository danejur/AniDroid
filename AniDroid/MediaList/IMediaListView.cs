using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.MediaModels;
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