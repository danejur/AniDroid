using System;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.AniListObject.Character
{
    public class CharacterPresenter : BaseAniDroidPresenter<ICharacterView>
    {
        public CharacterPresenter(ICharacterView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var characterId = View.GetCharacterId();
            var characterResp = await AniListService.GetCharacterById(characterId, default(CancellationToken));

            characterResp.Switch(character =>
                {
                    View.SetIsFavorite(character.IsFavourite);
                    View.SetShareText(character.Name?.GetFormattedName(), character.SiteUrl);
                    View.SetContentShown(false);
                    View.SetupToolbar($"{character.Name?.First} {character.Name?.Last}".Trim());
                    View.SetupCharacterView(character);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Media.Edge>> GetCharacterMediaEnumerable(int characterId, AniList.Models.Media.MediaType mediaType, int perPage)
        {
            return AniListService.GetCharacterMedia(characterId, mediaType, perPage);
        }
    }
}