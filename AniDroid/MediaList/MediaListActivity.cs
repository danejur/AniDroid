using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
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
using Ninject;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.MediaList
{
    [Activity(Label = "MediaListActivity")]
    public class MediaListActivity : BaseAniDroidActivity<MediaListPresenter>, IMediaListView
    {
        public const string UserIdIntentKey = "USER_ID";
        public const string MediaTypeIntentKey = "MEDIA_TYPE";

        [InjectView(Resource.Id.MediaLists_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.MediaLists_AppBar)]
        private AppBarLayout _appBar;
        [InjectView(Resource.Id.MediaLists_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.MediaLists_Tabs)]
        private TabLayout _tabLayout;
        [InjectView(Resource.Id.MediaLists_ViewPager)]
        private ViewPager _viewPager;

        private int _userId;
        private Media.MediaType _mediaType;
        private Media.MediaListCollection _collection;
        private IList<MediaListRecyclerAdapter> _recyclerAdapters;
        private MediaListSortComparer.MediaListSortType _currentSort;
        private MediaListSortComparer.MediaListSortDirection _currentSortDirection;
        private IMenu _menu;

        private IList<Media.MediaFormat> _filteredMediaFormats = new List<Media.MediaFormat>();
        private IList<Media.MediaStatus> _filteredMediaStatuses = new List<Media.MediaStatus>();
        private bool FilteringActive => _filteredMediaFormats?.Any() == true || _filteredMediaStatuses?.Any() == true;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IMediaListView, MediaListActivity>(this));

        public override void OnError(IAniListError error)
        {
            //throw new NotImplementedException();
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            //throw new NotImplementedException();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetLoadingShown();

            _userId = Intent.GetIntExtra(UserIdIntentKey, 0);
            _mediaType = AniListEnum.GetEnum<Media.MediaType>(Intent.GetStringExtra(MediaTypeIntentKey));

            await CreatePresenter(savedInstanceState);

            await Presenter.GetMediaLists(_userId);
        }

        public Media.MediaType GetMediaType()
        {
            return _mediaType;
        }

        public void SetCollection(Media.MediaListCollection collection)
        {
            SetContentView(Resource.Layout.Activity_MediaLists);

            _collection = collection;

            var pagerAdapter = new FragmentlessViewPagerAdapter();
            _recyclerAdapters = new List<MediaListRecyclerAdapter>();

            var listOrder = GetListOrder();
            var orderedLists = !listOrder.Any()
                ? _collection.Lists
                : _collection.Lists.Where(x =>
                        listOrder.All(y => y.Key != x.Name) || listOrder.FirstOrDefault(y => y.Key == x.Name).Value)
                    .OrderBy(x => listOrder.FindIndex(y => y.Key == x.Name)).ToList();

            _currentSort = Presenter.GetMediaListSortType(_mediaType);
            _currentSortDirection = Presenter.GetMediaListSortDirection(_mediaType);

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

                var adapter = new MediaListRecyclerAdapter(this, statusList, Presenter.GetCardType(),
                    item => MediaListViewModel.CreateViewModel(item, _collection.User.MediaListOptions.ScoreFormat,
                        Settings.DisplayUpcomingEpisodeTimeAsCountdown, Settings.MediaListProgressDisplay, true),
                    Presenter.GetMediaListItemViewType(), Presenter.GetHighlightPriorityItems(),
                    Presenter.GetUseLongClickForEpisodeAdd());

                _recyclerAdapters.Add(adapter);
                var listView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
                listView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView).SetAdapter(adapter);
                pagerAdapter.AddView(listView, statusList.Name);
            }

            _viewPager.Adapter = pagerAdapter;
            _tabLayout.SetupWithViewPager(_viewPager);

            SetupToolbar(_collection.User.Name);
        }

        public void UpdateMediaListItem(Media.MediaList mediaList)
        {
        }

        public void ResetMediaListItem(int mediaId)
        {
        }

        public void RemoveMediaListItem(int mediaListId)
        {
        }

        public static void StartActivity(BaseAniDroidActivity context, int userId, Media.MediaType mediaType)
        {
            var intent = new Intent(context, typeof(MediaListActivity));
            intent.PutExtra(UserIdIntentKey, userId);
            intent.PutExtra(MediaTypeIntentKey, mediaType.Value);
            context.StartActivity(intent);
        }

        private List<KeyValuePair<string, bool>> GetListOrder()
        {
            var settings = Kernel.Get<IAniDroidSettings>();
            var retList = new List<KeyValuePair<string, bool>>();

            if (_mediaType == Media.MediaType.Anime)
            {
                var lists = _collection.User.MediaListOptions?.AnimeList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.AnimeList.CustomLists ?? new List<string>()) ?? new List<string>();

                retList = settings.AnimeListOrder ?? lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
            }
            else if (_mediaType == Media.MediaType.Manga)
            {
                var lists = _collection.User.MediaListOptions?.MangaList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.MangaList.CustomLists ?? new List<string>()) ?? new List<string>();

                retList = settings.MangaListOrder ?? lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
            }

            return retList;
        }

        private void SetLoadingShown()
        {
            SetContentView(Resource.Layout.Activity_Loading);
            _coordLayout = FindViewById<CoordinatorLayout>(Resource.Id.Loading_CoordLayout);
            _toolbar = FindViewById<Toolbar>(Resource.Id.Loading_Toolbar);
            _toolbar.Title = "Loading";
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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

        #region Toolbar

        private void SetupToolbar(string userName)
        {
            if (_toolbar == null)
            {
                Logger.Warning("CheeseKnife", "CheeseKnife failed to bind _toolbar in MediaListActivity");
                _toolbar = FindViewById<Toolbar>(Resource.Id.MediaLists_Toolbar);
            }

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _toolbar.Title = $"{userName}'s {_mediaType.DisplayValue}";
        }

        public override bool SetupMenu(IMenu menu)
        {
            menu.Clear();
            var inflater = new MenuInflater(this);
            inflater.Inflate(Resource.Menu.MediaLists_ActionBar, menu);
            _menu = menu;

            _menu.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon
                ?.SetTintList(FilteringActive ? ColorStateList.ValueOf(Color.LightGreen) : null);

            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Ok);
                    Finish();
                    break;
                case Resource.Id.Menu_MediaLists_Sort:
                    MediaListSortDialog.Create(this, _currentSort, _currentSortDirection,
                        (sort, direction) =>
                        {
                            Presenter.SetMediaListSortSettings(_mediaType, sort, direction);
                            SetCollection(_collection);
                        });
                    break;
                case Resource.Id.Menu_MediaLists_Refresh:
                    Recreate();
                    break;
                case Resource.Id.Menu_MediaLists_Filter:
                    MediaListFilterDialog.Create(this, _mediaType, _filteredMediaStatuses, _filteredMediaFormats, UpdateAdapterFilters);
                    return true;
            }

            return true;
        }

        #endregion

    }
}