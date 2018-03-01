using AniDroid.Base;

namespace AniDroid.AniListObject.Character
{
    public interface ICharacterView : IAniListObjectView
    {
        int GetCharacterId();
        void SetupCharacterView(AniList.Models.Character character);
    }
}