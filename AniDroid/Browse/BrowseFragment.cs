using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Ninject;
using OneOf;

namespace AniDroid.Browse
{
    public class BrowseFragment : BaseMainActivityFragment<BrowsePresenter>, IBrowseView
    {
        public override string FragmentName => "BROWSE_FRAGMENT";

        private MediaRecyclerAdapter _adapter;
        private BaseRecyclerAdapter.RecyclerCardType _cardType;
        private static BrowseFragment _instance;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IBrowseView, BrowseFragment>(this));

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var recycler = View.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            _adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable, _cardType)
            {
                CreateViewModelFunc = MediaViewModel.CreateMediaViewModel
            };

            recycler.SetAdapter(_adapter);
        }

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
            _instance = instance as BrowseFragment;
        }

        public override void ClearState()
        {
            _instance = null;
        }

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
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

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            _adapter.RefreshAdapter();
        }
    }
}