using System;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;

namespace AniDroid.AniListObject.Character
{
    public class CharacterPresenter : BaseAniDroidPresenter<ICharacterView>
    {
        public CharacterPresenter(ICharacterView view, IAniListService service) : base(view, service)
        {
        }

        public override Task Init()
        {
            throw new NotImplementedException();
        }
    }
}