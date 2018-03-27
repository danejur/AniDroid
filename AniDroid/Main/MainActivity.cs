using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.User;
using AniDroid.Base;
using AniDroid.Browse;
using AniDroid.Dialogs;
using AniDroid.Discover;
using AniDroid.Home;
using AniDroid.Login;
using AniDroid.MediaList;
using AniDroid.SearchResults;
using AniDroid.Settings;
using AniDroid.TorrentSearch;
using AniDroid.Utils;
using AniDroid.Widgets;
using Ninject;
using OneOf;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Main
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : BaseAniDroidActivity<MainPresenter>, IMainView, NavigationView.IOnNavigationItemSelectedListener
    {
        public const string RecreateActivityIntentKey = "RECREATE_ACTIVITY";
        public const string NotificationTextIntentKey = "NOTIFICATION_TEXT";

        [InjectView(Resource.Id.Main_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Main_NavigationView)]
        private NavigationView _navigationView;
        [InjectView(Resource.Id.Main_DrawerLayout)]
        private DrawerLayout _navigationDrawer;
        [InjectView(Resource.Id.Main_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Main_SearchFab)]
        private FloatingActionButton _searchButton;

        private Toast _exitToast;
        private Action _navClosedAction;
        private BaseAniDroidFragment _currentFragment;
        private bool _fragmentBeingReplaced;
        private BadgeDrawerToggle _drawerToggle;
        private BadgeImageView _notificationImageView;
        private IMenuItem _selectedItem;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IMainView, MainActivity>(this));

        public override void OnError(IAniListError error)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public void SetAuthenticatedNavigationVisibility(bool isAuthenticated)
        {
            _navigationView?.Menu?.SetGroupVisible(Resource.Id.Menu_NavigationGroup_AuthenticatedUser, isAuthenticated);
        }

        public void OnMainViewSetup()
        {
            _searchButton.Clickable = true;
            _searchButton.Click -= SearchButtonOnClick;
            _searchButton.Click += SearchButtonOnClick;
            SelectDefaultFragment();
        }

        public void SetNotificationCount(int count)
        {
            var countVal = "";

            if (count > 99)
            {
                countVal = "99+";
            }
            else if (count > 0)
            {
                countVal = count.ToString();
            }

            _drawerToggle?.SetBadgeText(countVal);
            _notificationImageView?.SetText(countVal);
        }

        public void ShowSearchButton()
        {
            _searchButton?.Show();
        }

        private void SearchButtonOnClick(object sender, EventArgs eventArgs)
        {
            SearchDialog.Create(this, (type, term) => SearchResultsActivity.StartActivity(this, type, term));
        }

        public static void StartActivityForResult(Activity context, int requestCode, string notificationText = "")
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.PutExtra(NotificationTextIntentKey, notificationText ?? "");
            context.StartActivityForResult(intent, requestCode);
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Main);

            SetupToolbar();
            SetupNavigation();

            var notificationText = Intent.GetStringExtra(NotificationTextIntentKey);
            if (!string.IsNullOrWhiteSpace(notificationText))
            {
                DisplaySnackbarMessage(notificationText, Snackbar.LengthLong);
            }

            await CreatePresenter(savedInstanceState);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            if (Presenter != null)
            {
                await Presenter.GetUserNotificationCount();
            }
        }

        public override void OnBackPressed()
        {
            if (_exitToast == null || _exitToast.View.WindowVisibility != ViewStates.Visible)
            {
                _exitToast = Toast.MakeText(this, "Press back again to exit", ToastLength.Short);
                _exitToast.Show();
            }
            else
            {
                _exitToast.Cancel();
                Finish();
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data?.GetBooleanExtra(RecreateActivityIntentKey, false) == true)
            {
                _selectedItem = null;
                
                // TODO: probably a better way to do this, but this works for now
                SetTheme(GetThemeResource());

                SetContentView(Resource.Layout.Activity_Main);

                if (_currentFragment != null)
                {
                    SupportFragmentManager.BeginTransaction()
                        .Detach(_currentFragment)
                        .Attach(_currentFragment)
                        .CommitAllowingStateLoss();
                }

                SetupToolbar();
                SetupNavigation();

                await Presenter.Init();
            }
            else if (!string.IsNullOrWhiteSpace(data?.GetStringExtra(NotificationTextIntentKey)))
            {
                DisplaySnackbarMessage(data.GetStringExtra(NotificationTextIntentKey));
            }
        }

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _drawerToggle = new BadgeDrawerToggle(this, _navigationDrawer, _toolbar, 0, 0)
            {
                DrawerIndicatorEnabled = true,
                DrawerSlideAnimationEnabled = true
            };
            _drawerToggle.SetToggleColor(Color.White);

            _navigationDrawer.AddDrawerListener(_drawerToggle);
            _drawerToggle.SyncState();

        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                _navigationDrawer.OpenDrawer(GravityCompat.Start);
                return true;
            }

            return false;
        }

        #endregion

        #region Navigation

        private void SetupNavigation()
        {
            var navHeader = _navigationView.GetHeaderView(0);

            _notificationImageView = navHeader.FindViewById<BadgeImageView>(Resource.Id.Navigation_NotificationIcon);

            var userNameViewContainer = navHeader.FindViewById<LinearLayout>(Resource.Id.Navigation_UserNameContainer);
            var userNameView = navHeader.FindViewById<TextView>(Resource.Id.Navigation_UserName);
            if (!Settings.IsUserAuthenticated)
            {
                navHeader.FindViewById(Resource.Id.Navigation_NotificationIcon).Visibility = ViewStates.Gone;

                userNameViewContainer.Click += (sender, args) =>
                {
                    _navClosedAction = () =>
                    {
                        LoginActivity.StartActivity(this);
                    };
                    _navigationDrawer.CloseDrawer(GravityCompat.Start);
                };
            }
            else
            {
                var user = Settings.LoggedInUser;

                userNameView.Text = user?.Name ?? "User Error";
                userNameViewContainer.Click += (sender, args) =>
                {
                    _navClosedAction = () =>
                        AniListNotificationsDialog.Create(this, Presenter.GetNotificationsEnumerable(), () => SetNotificationCount(0));
                    _navigationDrawer.CloseDrawer(GravityCompat.Start);
                };

                if (!string.IsNullOrWhiteSpace(user?.Avatar?.Large))
                {
                    var profileImageView = navHeader.FindViewById<ImageView>(Resource.Id.Navigation_ProfileImage);
                    profileImageView.Visibility = ViewStates.Visible;
                    LoadImage(profileImageView, user.Avatar.Large);
                    profileImageView.Click += (sender, args) =>
                    {
                        _navClosedAction = () => UserActivity.StartActivity(this, user.Id);
                        _navigationDrawer.CloseDrawer(GravityCompat.Start);
                    };
                }

                if (!string.IsNullOrWhiteSpace(user?.BannerImage))
                {
                    var bannerView = navHeader.FindViewById<ImageView>(Resource.Id.Navigation_ProfileBannerImage);
                    bannerView.Visibility = ViewStates.Visible;
                    LoadImage(bannerView, user.BannerImage);
                }
            }

            _navigationView.SetNavigationItemSelectedListener(this);
            _navigationDrawer.DrawerClosed += OnDrawerClosed;
        }

        //this is needed to delay fragment replacement until the drawer is actually closed, so as not to show crappy animations
        private void OnDrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            if (_fragmentBeingReplaced)
            {
                ReplaceFragment();
            }
            else if (_navClosedAction != null)
            {
                var action = _navClosedAction;
                _navClosedAction = null;
                action.Invoke();
            }
        }

        private void ChangeFragment(BaseAniDroidFragment fragment)
        {
            var drawer = FindViewById<DrawerLayout>(Resource.Id.Main_DrawerLayout);
            drawer.CloseDrawer(GravityCompat.Start);

            if (fragment.FragmentName == _currentFragment?.FragmentName)
            {
                return;
            }

            _currentFragment = fragment;
            _fragmentBeingReplaced = true;
        }

        private void ReplaceFragment()
        {
            if (SupportFragmentManager.FindFragmentById(Resource.Id.Main_FragmentContainer) == null)
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.Main_FragmentContainer, _currentFragment)
                    .CommitAllowingStateLoss();
            else
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.Main_FragmentContainer, _currentFragment)
                    .CommitAllowingStateLoss();

            _fragmentBeingReplaced = false;
        }

        private void SelectDefaultFragment()
        {
            if (_currentFragment != null)
            {
                return;
            }

            _selectedItem = _selectedItem ?? _navigationView.Menu.FindItem(Presenter.GetIsUserAuthenticated()
                                   ? Resource.Id.Menu_Navigation_Anime
                                   : Resource.Id.Menu_Navigation_Discover);

            OnNavigationItemSelected(_selectedItem);
            _navigationView.SetCheckedItem(_selectedItem.ItemId);

            ReplaceFragment();
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            if (menuItem.ItemId == Resource.Id.Menu_Navigation_Settings)
            {
                _navClosedAction = () => SettingsActivity.StartActivity(this);
                _navigationDrawer.CloseDrawer(GravityCompat.Start);
                return false;
            }

            _selectedItem = menuItem;

            switch (_selectedItem.ItemId)
            {
                case Resource.Id.Menu_Navigation_Home:
                    ChangeFragment(HomeFragment.GetInstance() ?? new HomeFragment());
                    break;
                case Resource.Id.Menu_Navigation_Anime:
                    ChangeFragment(MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName) ??
                                   MediaListFragment.CreateMediaListFragment(Media.MediaType.Anime));
                    break;
                case Resource.Id.Menu_Navigation_Manga:
                    ChangeFragment(MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) ??
                                   MediaListFragment.CreateMediaListFragment(Media.MediaType.Manga));
                    break;
                case Resource.Id.Menu_Navigation_Discover:
                    ChangeFragment(new DiscoverFragment());
                    break;
                case Resource.Id.Menu_Navigation_TorrentSearch:
                    ChangeFragment(new TorrentSearchFragment());
                    break;
                case Resource.Id.Menu_Navigation_Browse:
                    ChangeFragment(new BrowseFragment());
                    break;
            }

            return true;
        }

        #endregion

    }
}