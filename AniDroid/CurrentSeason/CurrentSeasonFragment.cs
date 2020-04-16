using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils;
using AniDroid.Widgets;
using OneOf;

namespace AniDroid.CurrentSeason
{
    public class CurrentSeasonFragment : BaseMainActivityFragment<CurrentSeasonPresenter>, ICurrentSeasonView
    {
        private LinearLayout _listContainer;
        //private DiscoverMediaRecyclerAdapter _currentSeasonTvRecyclerAdapter;
        //private DiscoverMediaRecyclerAdapter _currentSeasonMoviewRecyclerAdapter;
        //private DiscoverMediaRecyclerAdapter _currentSeasonOvaOnaRecyclerAdapter;
        //private DiscoverMediaRecyclerAdapter _currentSeasonLeftoversRecyclerAdapter;

        private static CurrentSeasonFragment _instance;

        public override string FragmentName => "CURRENTSEASON_FRAGMENT";

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
            _instance = instance as CurrentSeasonFragment;
        }

        public override void ClearState()
        {
            _instance = null;
        }

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
        {
            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();

            var view = LayoutInflater.Inflate(Resource.Layout.Fragment_Discover, container, false);
            _listContainer = view.FindViewById<LinearLayout>(Resource.Id.Discover_Container);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            Presenter.GetCurrentSeasonLists();
        }

        public override void SetupMenu(IMenu menu)
        {
            menu.Clear();
            var inflater = new MenuInflater(Context);
            inflater.Inflate(Resource.Menu.Discover_ActionBar, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.Menu_Discover_Refresh)
            {
                //_currentSeasonTvRecyclerAdapter.ResetAdapter();

                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ShowCurrentTv(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            //var currentTvView = new SideScrollingList(Activity);
            //currentTvView.LabelText = "TV";
            //currentTvView.RecyclerAdapter = _currentSeasonTvRecyclerAdapter =
            //    new DiscoverMediaRecyclerAdapter(Activity, mediaEnumerable);

            //_listContainer.AddView(currentTvView);
        }

        public void ShowCurrentMovies(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            //throw new NotImplementedException();
        }

        public void ShowCurrentOvaOna(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            //throw new NotImplementedException();
        }

        public void ShowCurrentLeftovers(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            //throw new NotImplementedException();
        }
    }
}