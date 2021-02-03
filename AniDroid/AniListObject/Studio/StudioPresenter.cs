using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.AniListObject.Studio
{
    public class StudioPresenter : BaseAniDroidPresenter<IStudioView>
    {
        public StudioPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var studioId = View.GetStudioId();
            var studioResp = await AniListService.GetStudioById(studioId, default(CancellationToken));

            studioResp.Switch(studio =>
                {
                    View.SetIsFavorite(studio.IsFavourite);
                    View.SetShareText(studio.Name, studio.SiteUrl);
                    View.SetContentShown(false);
                    View.SetupToolbar(studio.Name);
                    View.SetupStudioView(studio);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStudioMediaEnumerable(int studioId, int perPage)
        {
            return AniListService.GetStudioMedia(studioId, perPage);
        }

        public async Task ToggleFavorite()
        {
            var studioId = View.GetStudioId();
            var favResp = await AniListService.ToggleFavorite(new FavoriteDto { StudioId = studioId },
                default(CancellationToken));

            favResp.Switch(error => View.OnError(error))
                .Switch(favorites => View.SetIsFavorite(favorites.Studios?.Nodes?.Any(x => x.Id == studioId) == true, true));
        }
    }
}