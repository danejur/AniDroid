using AniDroid.Base;

namespace AniDroid.AniListObject.Studio
{
    public interface IStudioView : IAniListObjectView
    {
        int GetStudioId();
        void SetupStudioView(AniList.Models.StudioModels.Studio studio);
    }
}