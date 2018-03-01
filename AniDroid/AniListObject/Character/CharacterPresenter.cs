using System;
using System.Threading;
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

        public override async Task Init()
        {
            var characterId = View.GetCharacterId();
            var characterResp = await AniListService.GetCharacterById(characterId, default(CancellationToken));

            characterResp.Switch(character =>
                {
                    View.SetIsFavorite(character.IsFavourite);
                    View.SetShareText(character.Name?.GetFormattedName(), character.SiteUrl);
                    View.SetContentShown();
                    View.SetupToolbar($"{character.Name?.First} {character.Name?.Last}".Trim());
                    View.SetupCharacterView(character);
                })
                .Switch(error => View.OnNetworkError());
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Media.Edge>> GetCharacterMediaEnumerable(int characterId, AniList.Models.Media.MediaType mediaType, int perPage)
        {
            return AniListService.GetCharacterMedia(characterId, mediaType, perPage);
        }
    }
}