using AniDroid.Base;

namespace AniDroid.AniListObject.Media
{
    public interface IMediaView : IAniListObjectView
    {
        int GetMediaId();
        AniList.Models.Media.MediaType GetMediaType();
        void SetupMediaView(AniList.Models.Media media);
        void SetupMediaFab(AniList.Models.Media media);
    }
}