using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.Search;
using AniDroid.Adapters.SearchAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.SearchResults
{
    [Activity(Label = "Search Results")]
    public class SearchResultsActivity : BaseAniDroidActivity<SearchResultsPresenter>, ISearchResultsView
    {
        private string _searchType;
        private string _searchTerm;

        [InjectView(Resource.Id.SearchResults_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.SearchResults_RecyclerView)]
        private RecyclerView _recyclerView;
        [InjectView(Resource.Id.SearchResults_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.SearchResults_SearchFab)]
        private FloatingActionButton _searchButton;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<ISearchResultsView, SearchResultsActivity>(this));

        public override void OnNetworkError()
        {
            throw new NotImplementedException();
        }

        public void ShowMediaSearchResults(IAsyncEnumerable<IPagedData<Media>> mediaEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new MediaSearchRecyclerAdapter(this, mediaEnumerable,
                BaseRecyclerAdapter.CardType.Vertical));
        }

        public void ShowCharacterSearchResults(IAsyncEnumerable<IPagedData<Character>> characterEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new CharacterSearchRecyclerAdapter(this, characterEnumerable,
                BaseRecyclerAdapter.CardType.Vertical));
        }

        public void ShowStaffSearchResults(IAsyncEnumerable<IPagedData<Staff>> staffEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new StaffSearchRecyclerAdapter(this, staffEnumerable,
                BaseRecyclerAdapter.CardType.Vertical));
        }

        public void ShowUserSearchResults(IAsyncEnumerable<IPagedData<User>> userEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new UserSearchRecyclerAdapter(this, userEnumerable,
                BaseRecyclerAdapter.CardType.Vertical));
        }

        public void ShowForumThreadSearchResults(IAsyncEnumerable<IPagedData<ForumThread>> forumThreadEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new ForumThreadSearchRecyclerAdapter(this, forumThreadEnumerable));
        }

        public void ShowStudioSearchResults(IAsyncEnumerable<IPagedData<Studio>> studioEnumerable)
        {
            // TODO: CardType should be fetched from settings
            _recyclerView.SetAdapter(new StudioSearchRecyclerAdapter(this, studioEnumerable));
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_SearchResults);

            _searchType = Intent.GetStringExtra(IntentKeys.SearchType);
            _searchTerm = Intent.GetStringExtra(IntentKeys.SearchTerm);

            await CreatePresenter(savedInstanceState);
            Presenter.SearchAniList(_searchType, _searchTerm);

            _searchButton.Clickable = true;
            _searchButton.Click -= SearchButtonOnClick;
            _searchButton.Click += SearchButtonOnClick;

            SetupToolbar();
        }

        private void SearchButtonOnClick(object sender, EventArgs eventArgs)
        {
            SearchDialog.Create(this, (type, term) =>
            {
                _searchType = type;
                _searchTerm = term;
                Presenter.SearchAniList(type, term);
            }, _searchType, _searchTerm);
        }

        public static void StartActivity(Context context, string type, string term)
        {
            var searchIntent = new Intent(context, typeof(SearchResultsActivity));
            searchIntent.PutExtra(IntentKeys.SearchTerm, term);
            searchIntent.PutExtra(IntentKeys.SearchType, type);
            context.StartActivity(searchIntent);
        }

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            return false;
        }

        #endregion

        #region Constants

        public static class IntentKeys
        {
            public const string SearchType = "SEARCH_TYPE";
            public const string SearchTerm = "SEARCH_TERM";
        }

        public static class AniListSearchTypes
        {
            public const string Anime = "Anime";
            public const string Characters = "Characters";
            public const string Staff = "Staff";
            public const string Manga = "Manga";
            public const string Studios = "Studios";
            public const string Users = "Users";
            public const string Forum = "Forum";

            public static string[] AllTypes => new[] { Anime, Characters, Staff, Manga, Studios, Users, Forum };
        }

        #endregion
    }
}