using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Utils;
using AniDroid.Utils.Comparers;
using AniDroid.Utils.Interfaces;

namespace AniDroid.MediaList
{
    public class MediaListFragment : BaseMainActivityFragment<MediaListPresenter>, IMediaListView
    {
        public const string AnimeMediaListFragmentName = "ANIME_MEDIA_LIST_FRAGMENT";
        public const string MangaMediaListFragmentName = "MANGA_MEDIA_LIST_FRAGMENT";
        private const string MediaTypeKey = "MEDIA_TYPE";
        private const string MediaSortKey = "MEDIA_SORT";
        private const string UserIdKey = "USER_ID";

        private int _userId;
        private Media.MediaType _type;
        private IList<MediaListRecyclerAdapter> _recyclerAdapters;
        private Media.MediaListCollection _collection;
        private MediaListSortComparer.MediaListSortType _currentSort;
        private MediaListSortComparer.SortDirection _currentSortDirection;
        private IMenu _menu;

        private IList<Media.MediaFormat> _filteredMediaFormats = new List<Media.MediaFormat>();
        private IList<Media.MediaStatus> _filteredMediaStatuses = new List<Media.MediaStatus>();
        private bool FilteringActive => _filteredMediaFormats?.Any() == true || _filteredMediaStatuses?.Any() == true;

        private static MediaListFragment _animeListFragmentInstance;
        private static MediaListFragment _mangaListFragmentInstance;

        public override bool HasMenu => true;
        public override string FragmentName {
            get {
                if (_type == Media.MediaType.Anime)
                {
                    return AnimeMediaListFragmentName;
                }

                return _type == Media.MediaType.Manga ? MangaMediaListFragmentName : "";
            }
        }

        public static BaseMainActivityFragment<MediaListPresenter> GetInstance(string fragmentName)
        {
            switch (fragmentName)
            {
                case AnimeMediaListFragmentName:
                    return _animeListFragmentInstance;
                case MangaMediaListFragmentName:
                    return _mangaListFragmentInstance;
                default:
                    return null;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var typeString = Arguments.GetString(MediaTypeKey);
            _type = AniListEnum.GetEnum<Media.MediaType>(typeString);
            _userId = Arguments.GetInt(UserIdKey);

            if (_type == Media.MediaType.Anime)
            {
                _animeListFragmentInstance = this;
            }
            else if (_type == Media.MediaType.Manga)
            {
                _mangaListFragmentInstance = this;
            }
        }

        public static MediaListFragment CreateMediaListFragment(int userId, Media.MediaType type, Media.MediaSort sort = null)
        {
            var frag = new MediaListFragment();
            var bundle = new Bundle(6);
            bundle.PutString(MediaTypeKey, type.Value);
            bundle.PutInt(UserIdKey, userId);
            frag.Arguments = bundle;

            return frag;
        }

        public override void OnError(IAniListError error)
        {
            // TODO: show error fragment here
        }

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
        }

        public override void ClearState()
        {
            if (_type == Media.MediaType.Anime)
            {
                _animeListFragmentInstance = null;
            }
            else if (_type == Media.MediaType.Manga)
            {
                _mangaListFragmentInstance = null;
            }
        }

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
        {
            if (_collection != null)
            {
                return GetMediaListCollectionView();
            }
            
            if (_type == null)
            {
                return LayoutInflater.Inflate(Resource.Layout.View_Error, container, false);
            }

            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            Presenter.GetMediaLists(_userId);

            return LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, container, false);
        }

        public Media.MediaType GetMediaType()
        {
            return _type;
        }

        public void SetCollection(Media.MediaListCollection collection)
        {
            _collection = collection;
            RecreateFragment();
        }

        public void UpdateMediaListItem(Media.MediaList mediaList)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                if (mediaList?.Media?.Id == null)
                {
                    Activity.Logger.Error("UpdateMediaListItem", "A media list item update was attempted with a null reference.");
                    continue;
                }

                adapter.UpdateMediaListItem(mediaList.Media.Id, mediaList);
            }
        }

        public void ResetMediaListItem(int mediaId)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                adapter.ResetMediaListItem(mediaId);
            }
        }

        public void RemoveMediaListItem(int mediaListId)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                adapter.RemoveMediaListItem(mediaListId);
            }
        }

        public void SetMediaListSort(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.SortDirection direction)
        {
            _currentSort = sort;
            _currentSortDirection = direction;

            Presenter.SetMediaListSortSettings(_type, _currentSort, _currentSortDirection);

            RecreateFragment();
        }

        public override void SetupMenu(IMenu menu)
        {
            menu.Clear();
            var inflater = new MenuInflater(Context);
            inflater.Inflate(Resource.Menu.MediaLists_ActionBar, menu);
            _menu = menu;

            _menu.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon
                ?.SetTintList(FilteringActive ? ColorStateList.ValueOf(Color.LightGreen) : null);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Menu_MediaLists_Refresh:
                    _collection = null;
                    RecreateFragment();
                    return true;
                case Resource.Id.Menu_MediaLists_Sort:
                    MediaListSortDialog.Create(Activity, _currentSort, _currentSortDirection, SetMediaListSort);
                    return true;
                case Resource.Id.Menu_MediaLists_Filter:
                    MediaListFilterDialog.Create(Activity, _type, _filteredMediaStatuses, _filteredMediaFormats, UpdateAdapterFilters);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void UpdateAdapterFilters(IList<Media.MediaStatus> statusList,
            IList<Media.MediaFormat> formatList)
        {
            _filteredMediaFormats = formatList ?? new List<Media.MediaFormat>();
            _filteredMediaStatuses = statusList ?? new List<Media.MediaStatus>();

            if (_filteredMediaStatuses.Any() || _filteredMediaFormats.Any())
            {
                DisplaySnackbarMessage("List filtering is active", Snackbar.LengthLong);
                _menu?.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon?.SetTintList(ColorStateList.ValueOf(Color.LightGreen));
            }
            else
            {
                DisplaySnackbarMessage("List filtering is not active", Snackbar.LengthShort);
                _menu?.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon?.SetTintList(null);
            }

            foreach (var adapter in _recyclerAdapters)
            {
                adapter.UpdateFilters(_filteredMediaFormats, _filteredMediaStatuses);
            }
        }

        private View GetMediaListCollectionView()
        {
            var mediaCollectionView = LayoutInflater.Inflate(Resource.Layout.Fragment_MediaLists, null);
            var pagerAdapter = new FragmentlessViewPagerAdapter();
            _recyclerAdapters = new List<MediaListRecyclerAdapter>();

            var listOrder = GetListOrder();
            var orderedLists = _collection.Lists
                .Where(x => listOrder.FirstOrDefault(y => y.Key == x.Name).Value)
                .OrderBy(x => listOrder.FindIndex(y => y.Key == x.Name)).ToList();

            _currentSort = Presenter.GetMediaListSortType(_type);
            _currentSortDirection = Presenter.GetMediaListSortDirection(_type);

            if (_currentSort != MediaListSortComparer.MediaListSortType.NoSort)
            {
                _collection.Lists.ForEach(list =>
                    list.Entries.Sort(new MediaListSortComparer(_currentSort, _currentSortDirection)));
            }

            foreach (var statusList in orderedLists)
            {
                if (statusList.Entries?.Any() != true)
                {
                    continue;
                }

                var adapter = new MediaListRecyclerAdapter(Activity, statusList, Presenter.GetCardType(),
                    item => MediaListViewModel.CreateViewModel(item, _collection.User.MediaListOptions.ScoreFormat,
                        Presenter.GetDisplayTimeUntilAiringAsCountdown(), Presenter.GetProgressDisplayType(), false,
                        Presenter.GetShowEpisodeAddButtonForRepeatingMedia()),
                    Presenter.GetMediaListItemViewType(), Presenter.GetHighlightPriorityItems(),
                    Presenter.GetUseLongClickForEpisodeAdd(),
                    async (viewModel, callback) =>
                    {
                        if (viewModel.Model.Progress + 1 ==
                            (viewModel.Model.Media.Episodes ?? viewModel.Model.Media.Chapters))
                        {
                            EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model.Media, viewModel.Model,
                                _collection.User.MediaListOptions, true);
                        }
                        else
                        {
                            await Presenter.IncreaseMediaProgress(viewModel.Model);
                        }

                        callback?.Invoke();
                    })
                {
                    LongClickAction = (viewModel, position) => EditMediaListItemDialog.Create(Activity, Presenter,
                        viewModel.Model.Media, viewModel.Model, _collection.User.MediaListOptions)
                };

                adapter.UpdateFilters(_filteredMediaFormats, _filteredMediaStatuses);

                _recyclerAdapters.Add(adapter);
                var listView = LayoutInflater.Inflate(Resource.Layout.View_SwipeRefreshList, null);
                listView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView).SetAdapter(adapter);
                pagerAdapter.AddView(listView, statusList.Name);

                var swipeRefreshLayout = listView.FindViewById<SwipeRefreshLayout>(Resource.Id.List_SwipeRefreshLayout);

                if (Presenter.GetUseSwipeToRefreshOnMediaLists())
                {
                    swipeRefreshLayout.Enabled = true;
                    swipeRefreshLayout.Refresh += (sender, e) =>
                    {
                        _collection = null;
                        RecreateFragment();
                        if (sender is SwipeRefreshLayout refreshLayout)
                        {
                            refreshLayout.Refreshing = false;
                        }
                    };
                }
                else
                {
                    swipeRefreshLayout.Enabled = false;
                }
            }

            var viewPagerView = mediaCollectionView.FindViewById<ViewPager>(Resource.Id.MediaLists_ViewPager);
            viewPagerView.Adapter = pagerAdapter;
            mediaCollectionView.FindViewById<TabLayout>(Resource.Id.MediaLists_Tabs).SetupWithViewPager(viewPagerView);

            return mediaCollectionView;
        }

        private List<KeyValuePair<string, bool>> GetListOrder()
        {
            var retList = new List<KeyValuePair<string, bool>>();

            if (_type == Media.MediaType.Anime)
            {
                var lists = _collection.User.MediaListOptions?.AnimeList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.AnimeList.CustomLists ?? new List<string>()) ?? new List<string>();

                if (Presenter.AniDroidSettings.AnimeListOrder?.Any() != true)
                {
                    // if we don't have the list order yet, go ahead and store it
                    Presenter.AniDroidSettings.AnimeListOrder = lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
                }

                retList = Presenter.AniDroidSettings.AnimeListOrder;
            }
            else if (_type == Media.MediaType.Manga)
            {
                var lists = _collection.User.MediaListOptions?.MangaList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.MangaList.CustomLists ?? new List<string>()) ?? new List<string>();

                if (Presenter.AniDroidSettings.MangaListOrder?.Any() != true)
                {
                    Presenter.AniDroidSettings.MangaListOrder = lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
                }

                retList = Presenter.AniDroidSettings.MangaListOrder;
            }

            return retList;
        }
    }
}