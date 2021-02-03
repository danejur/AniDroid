using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.TorrentSearch
{
    public class TorrentSearchPresenter : BaseAniDroidPresenter<ITorrentSearchView>
    {
        public TorrentSearchPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void SearchNyaaSi(NyaaSiSearchRequest searchReq)
        {
            View.ShowNyaaSiSearchResults(NyaaSiService.GetSearchEnumerable(searchReq));
        }
    }
}