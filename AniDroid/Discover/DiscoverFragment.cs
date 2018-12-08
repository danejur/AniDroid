using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using AniDroid.Widgets;
using Ninject;
using OneOf;

namespace AniDroid.Discover
{
    public class DiscoverFragment : BaseMainActivityFragment<DiscoverPresenter>, IDiscoverView
    {
        private LinearLayout _listContainer;
        private MediaRecyclerAdapter _currentlyAiringRecyclerAdapter;
        private MediaRecyclerAdapter _trendingRecyclerAdapter;
        private MediaRecyclerAdapter _newAnimeRecyclerAdapter;
        private MediaRecyclerAdapter _newMangaRecyclerAdapter;

        private List<MediaRecyclerAdapter> AdapterList => new List<MediaRecyclerAdapter>
        {
            _currentlyAiringRecyclerAdapter,
            _trendingRecyclerAdapter,
            _newAnimeRecyclerAdapter,
            _newMangaRecyclerAdapter
        };

        private static DiscoverFragment _instance;

        private const int CardWidth = 150;

        public override bool HasMenu => true;
        public override string FragmentName => "DISCOVER_FRAGMENT";
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IDiscoverView, DiscoverFragment>(this));

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
            _instance = instance as DiscoverFragment;
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
            Presenter.GetDiscoverLists();
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
                _currentlyAiringRecyclerAdapter.ResetAdapter();
                _trendingRecyclerAdapter.ResetAdapter();
                _newAnimeRecyclerAdapter.ResetAdapter();
                _newMangaRecyclerAdapter.ResetAdapter();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentlyAiringResults(
            IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var currentlyAiringView = new SideScrollingList(Activity) {LabelText = "Currently Airing"};

            var adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable,
                BaseRecyclerAdapter.RecyclerCardType.Vertical, MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = viewModel =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model,
                            viewModel.Model.MediaListEntry,
                            Presenter.GetLoggedInUser()?.MediaListOptions);
                    }
                },

            };
            adapter.SetHorizontalAdapterCardWidthDip(CardWidth);

            currentlyAiringView.RecyclerAdapter = _currentlyAiringRecyclerAdapter = adapter;

            _listContainer.AddView(currentlyAiringView);
        }

        public void ShowTrendingResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var trendingView = new SideScrollingList(Activity) {LabelText = "Trending"};

            var adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable,
                BaseRecyclerAdapter.RecyclerCardType.Vertical, MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = viewModel =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model,
                            viewModel.Model.MediaListEntry,
                            Presenter.GetLoggedInUser()?.MediaListOptions);
                    }
                },

            };
            adapter.SetHorizontalAdapterCardWidthDip(CardWidth);

            trendingView.RecyclerAdapter = _trendingRecyclerAdapter = adapter;

            _listContainer.AddView(trendingView);
        }

        public void ShowNewAnimeResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var newAnimeView = new SideScrollingList(Activity) {LabelText = "New Anime"};

            var adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable,
                BaseRecyclerAdapter.RecyclerCardType.Vertical, MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = viewModel =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model,
                            viewModel.Model.MediaListEntry,
                            Presenter.GetLoggedInUser()?.MediaListOptions);
                    }
                },

            };
            adapter.SetHorizontalAdapterCardWidthDip(CardWidth);

            newAnimeView.RecyclerAdapter = _newAnimeRecyclerAdapter = adapter;

            _listContainer.AddView(newAnimeView);
        }

        public void ShowNewMangaResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var newMangaView = new SideScrollingList(Activity) {LabelText = "New Manga"};

            var adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable,
                BaseRecyclerAdapter.RecyclerCardType.Vertical, MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = viewModel =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model,
                            viewModel.Model.MediaListEntry,
                            Presenter.GetLoggedInUser()?.MediaListOptions);
                    }
                },

            };
            adapter.SetHorizontalAdapterCardWidthDip(CardWidth);

            newMangaView.RecyclerAdapter = _newMangaRecyclerAdapter = adapter;

            _listContainer.AddView(newMangaView);
        }

        public void UpdateMediaListItem(Media.MediaList mediaList)
        {
            if (mediaList.Media?.Type == Media.MediaType.Anime)
            {
                var instance = MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName);

                (instance as MediaListFragment)?.UpdateMediaListItem(mediaList);
            }
            else if (mediaList.Media?.Type == Media.MediaType.Manga)
            {
                (MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) as MediaListFragment)
                    ?.UpdateMediaListItem(mediaList);
            }

            UpdateMediaListOnAdapters(mediaList);
        }

        public void RemoveMediaListItem(int mediaListId)
        {
            (MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName) as MediaListFragment)
                ?.RemoveMediaListItem(mediaListId);
            (MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) as MediaListFragment)
                ?.RemoveMediaListItem(mediaListId);

            DeleteMediaListOnAdapters(mediaListId);
        }

        private void UpdateMediaListOnAdapters(Media.MediaList mediaList)
        {
            AdapterList.ForEach(adapter => {
                var itemPosition =
                    adapter?.Items.FindIndex(x => x?.Model?.Id == mediaList.Media?.Id);

                if (itemPosition >= 0)
                {
                    mediaList.Media.MediaListEntry = mediaList;

                    adapter.ReplaceItem(itemPosition.Value, adapter.CreateViewModelFunc?.Invoke(mediaList.Media));
                }
            });
        }

        private void DeleteMediaListOnAdapters(int mediaListId)
        {
            AdapterList.ForEach(adapter => {
                var itemPosition =
                    adapter?.Items?.FindIndex(x => x?.Model?.MediaListEntry?.Id == mediaListId);

                if (itemPosition >= 0)
                {
                    var item = adapter.Items[itemPosition.Value];
                    item.Model.MediaListEntry = null;

                    adapter.ReplaceItem(itemPosition.Value, adapter.CreateViewModelFunc?.Invoke(item.Model));
                }
            });
            
        }
    }
}