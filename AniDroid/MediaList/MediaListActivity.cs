using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Utils;
using AniDroid.Utils.Comparers;
using Google.Android.Material.AppBar;
using Google.Android.Material.Snackbar;
using Google.Android.Material.Tabs;

namespace AniDroid.MediaList
{
    [Activity(Label = "MediaListActivity")]
    public class MediaListActivity : BaseAniDroidActivity<MediaListPresenter>, IMediaListView
    {
        public const string UserIdIntentKey = "USER_ID";
        public const string MediaTypeIntentKey = "MEDIA_TYPE";
        private const int FilterTextUpdateHandlerMessage = 1;

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
        [InjectView(Resource.Id.MediaLists_ToolbarSearch)]
        private EditText _toolbarSearch;

        private int _userId;
        private MediaType _mediaType;
        private MediaListCollection _collection;
        private IList<MediaListRecyclerAdapter> _recyclerAdapters;
        private MediaListSortComparer.MediaListSortType _currentSort;
        private MediaListSortComparer.SortDirection _currentSortDirection;
        private IMenu _menu;
        private MediaListFilterModel _filterModel;
        private Handler _filterTextHandler;

        public override void OnError(IAniListError error)
        {
            //throw new NotImplementedException();
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            if (_coordLayout != null)
            {
                Snackbar.Make(_coordLayout, message, length).Show();
            }
            else
            {
                // as a fallback (if the coord layout is null for some reason), show a toast
                Toast.MakeText(this, message, ToastLength.Long);
            }
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetLoadingShown();

            _userId = Intent.GetIntExtra(UserIdIntentKey, 0);
            _mediaType = AniListEnum.GetEnum<MediaType>(Intent.GetStringExtra(MediaTypeIntentKey));

            _filterModel = new MediaListFilterModel();

            await CreatePresenter(savedInstanceState);

            await Presenter.GetMediaLists(_userId);


            _filterTextHandler = new Handler(UpdateFilterText);

            _toolbarSearch.AfterTextChanged -= ToolbarSearchTextChanged;
            _toolbarSearch.AfterTextChanged += ToolbarSearchTextChanged;
        }

        public MediaType GetMediaType()
        {
            return _mediaType;
        }

        private void ToolbarSearchTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            _filterTextHandler.RemoveMessages(FilterTextUpdateHandlerMessage);
            _filterTextHandler.SendEmptyMessageDelayed(FilterTextUpdateHandlerMessage, 500);
        }

        private void UpdateFilterText(Message message)
        {
            if (_toolbarSearch?.Text != null && !string.Equals(_filterModel.Title, _toolbarSearch.Text))
            {
                _filterModel.Title = _toolbarSearch.Text;
                SetMediaListFilter(_filterModel);
            }
        }

        public void SetCollection(MediaListCollection collection)
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
                        Settings.DisplayUpcomingEpisodeTimeAsCountdown, Settings.MediaListProgressDisplay, true,
                        Settings.ShowEpisodeAddButtonForRepeatingMedia),
                    Presenter.GetMediaListItemViewType(), Presenter.GetHighlightPriorityItems(),
                    Presenter.GetUseLongClickForEpisodeAdd());

                adapter.SetFilter(_filterModel);

                _recyclerAdapters.Add(adapter);
                var listView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
                listView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView).SetAdapter(adapter);
                pagerAdapter.AddView(listView, statusList.Name);
            }

            _viewPager.Adapter = pagerAdapter;
            _tabLayout.SetupWithViewPager(_viewPager);

            SetupToolbar(_collection.User.Name);
        }

        public void UpdateMediaListItem(AniList.Models.MediaModels.MediaList mediaList)
        {
        }

        public void ResetMediaListItem(int mediaId)
        {
        }

        public void RemoveMediaListItem(int mediaListId)
        {
        }

        public MediaListFilterModel GetMediaListFilter()
        {
            return _filterModel;
        }

        public static void StartActivity(BaseAniDroidActivity context, int userId, MediaType mediaType)
        {
            var intent = new Intent(context, typeof(MediaListActivity));
            intent.PutExtra(UserIdIntentKey, userId);
            intent.PutExtra(MediaTypeIntentKey, mediaType.Value);
            context.StartActivity(intent);
        }

        private List<KeyValuePair<string, bool>> GetListOrder()
        {
            var retList = new List<KeyValuePair<string, bool>>();

            if (_mediaType == MediaType.Anime)
            {
                var lists = _collection.User.MediaListOptions?.AnimeList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.AnimeList.CustomLists ?? new List<string>()) ?? new List<string>();

                retList = Presenter.AniDroidSettings.AnimeListOrder ?? lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
            }
            else if (_mediaType == MediaType.Manga)
            {
                var lists = _collection.User.MediaListOptions?.MangaList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.MangaList.CustomLists ?? new List<string>()) ?? new List<string>();

                retList = Presenter.AniDroidSettings.MangaListOrder ?? lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
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

        public void SetMediaListFilter(MediaListFilterModel filterModel)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                adapter.SetFilter(filterModel);
            }

            if (!string.Equals(_filterModel.Title, _toolbarSearch.Text))
            {
                _toolbarSearch.Text = _filterModel.Title;
            }

            if (_filterModel.IsFilteringActive)
            {
                if (!_filterModel.FilteringPreviouslyActive)
                {
                    DisplaySnackbarMessage("List filtering is active", Snackbar.LengthLong);
                }

                _filterModel.FilteringPreviouslyActive = true;
                _menu?.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon
                    ?.SetTintList(ColorStateList.ValueOf(Color.LightGreen));
            }
            else
            {
                if (_filterModel.FilteringPreviouslyActive)
                {
                    DisplaySnackbarMessage("List filtering is not active", Snackbar.LengthShort);
                }

                _filterModel.FilteringPreviouslyActive = false;
                _menu?.FindItem(Resource.Id.Menu_MediaLists_Filter)?.Icon?.SetTintList(null);
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
                ?.SetTintList(_filterModel.IsFilteringActive ? ColorStateList.ValueOf(Color.LightGreen) : null);

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
                    MediaListFilterDialog.Create(this, this, _mediaType, Presenter.GetGenres(), Presenter.GetMediaTags());
                    return true;
            }

            return true;
        }

        #endregion

    }
}