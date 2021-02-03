using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.UserModels;
using AniDroid.Base;

namespace AniDroid.AniListObject.Media
{
    public interface IMediaView : IAniListObjectView
    {
        int GetMediaId();
        MediaType GetMediaType();
        void SetCanEditListItem();
        void SetupMediaView(AniList.Models.MediaModels.Media media);
        void SetCurrentUserMediaListOptions(UserMediaListOptions mediaListOptions);
        void ShowMediaListEditDialog(AniList.Models.MediaModels.MediaList mediaList);
        void UpdateMediaListItem(AniList.Models.MediaModels.MediaList mediaList);
        void RemoveMediaListItem();
    }
}