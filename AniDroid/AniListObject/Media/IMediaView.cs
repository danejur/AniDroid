using AniDroid.Base;

namespace AniDroid.AniListObject.Media
{
    public interface IMediaView : IAniListObjectView
    {
        int GetMediaId();
        AniList.Models.Media.MediaType GetMediaType();
        void SetCanEditListItem();
        void SetMediaListSaving();
        void SetupMediaView(AniList.Models.Media media);
        void SetCurrentUserMediaListOptions(AniList.Models.User.UserMediaListOptions mediaListOptions);
        void ShowMediaListEditDialog(AniList.Models.Media.MediaList mediaList);
        void UpdateMediaListItem(AniList.Models.Media.MediaList mediaList);
    }
}