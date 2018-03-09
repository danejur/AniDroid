using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Ninject;

namespace AniDroid.Browse
{
    public class BrowseFragment : BaseAniDroidFragment<BrowsePresenter>, IBrowseView
    {
        public override string FragmentName => "BROWSE_FRAGMENT";

        private BaseRecyclerAdapter.CardType _cardType;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IBrowseView, BrowseFragment>(this));

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public void ShowMediaSearchResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable)
        {
            var recycler = View.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            recycler.SetAdapter(new BrowseMediaRecyclerAdapter(Activity, mediaEnumerable, _cardType));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            var settings = Kernel.Get<IAniDroidSettings>();
            _cardType = settings.CardType;

            return LayoutInflater.Inflate(Resource.Layout.View_List, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            Presenter.BrowseAniListMedia(new BrowseMediaDto());
        }
    }
}