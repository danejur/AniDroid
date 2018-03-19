using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.TorrentSearch
{
    public class TorrentSearchFragment : BaseAniDroidFragment<TorrentSearchPresenter>, ITorrentSearchView
    {
        public override string FragmentName => "TORRENT_SEARCH_FRAGMENT";

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ITorrentSearchView, TorrentSearchFragment>(this));

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public override View CreateView(ViewGroup container, Bundle savedInstanceState)
        {
            return LayoutInflater.Inflate(Resource.Layout.Fragment_NotImplemented, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            await CreatePresenter(savedInstanceState);
        }
    }
}