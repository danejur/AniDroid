using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.AniListObject.Character
{
    public class CharacterPresenter : BaseAniDroidPresenter<ICharacterView>
    {
        public CharacterPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
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

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Media.Edge>, IAniListError>> GetCharacterMediaEnumerable(int characterId, AniList.Models.Media.MediaType mediaType, int perPage)
        {
            return AniListService.GetCharacterMedia(characterId, mediaType, perPage);
        }

        public async Task ToggleFavorite()
        {
            var characterId = View.GetCharacterId();
            var favResp = await AniListService.ToggleFavorite(new FavoriteDto {CharacterId = characterId},
                default(CancellationToken));

            favResp.Switch(error => View.OnError(error))
                .Switch(favorites => View.SetIsFavorite(favorites.Characters?.Nodes?.Any(x => x.Id == characterId) == true, true));
        }
    }
}