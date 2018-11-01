using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Torrent.NyaaSi;
using AniDroid.Utils.Interfaces;

namespace AniDroid.TorrentSearch
{
    public class TorrentSearchPresenter : BaseAniDroidPresenter<ITorrentSearchView>
    {
        public TorrentSearchPresenter(ITorrentSearchView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
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