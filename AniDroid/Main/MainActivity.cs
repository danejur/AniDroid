using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.SearchResults;
using AniDroid.Settings;
using AniDroid.Utils;
using Ninject;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Main
{
    [Activity(Label = "AniDroid", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize, LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : BaseAniDroidActivity<MainPresenter>, IMainView
    {
        [InjectView(Resource.Id.Main_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Main_NavigationView)]
        private NavigationView _navigationView;
        [InjectView(Resource.Id.Main_DrawerLayout)]
        private DrawerLayout _navDrawer;
        [InjectView(Resource.Id.Main_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Main_SearchFab)]
        private FloatingActionButton _searchButton;

        private Toast _exitToast;
        private Action _navClosedAction;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IMainView, MainActivity>(this));

        public override void OnError(IAniListError error)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public void SetAuthenticatedNavigationVisibility(bool isAuthenticated)
        {
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Anime)?.SetVisible(isAuthenticated);
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Manga)?.SetVisible(isAuthenticated);
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Home)?.SetVisible(isAuthenticated);
        }

        public void OnMainViewSetup()
        {
            _searchButton.Clickable = true;
            _searchButton.Click -= SearchButtonOnClick;
            _searchButton.Click += SearchButtonOnClick;
        }

        private void SearchButtonOnClick(object sender, EventArgs eventArgs)
        {
            SearchDialog.Create(this, (type, term) => SearchResultsActivity.StartActivity(this, type, term));
        }

        public static void StartActivity(Activity context)
        {
            var intent = new Intent(context, typeof(MainActivity));
            context.StartActivity(intent);
        }

        public static void StartActivityForResult(Activity context, int requestCode)
        {
            var intent = new Intent(context, typeof(MainActivity));
            context.StartActivityForResult(intent, requestCode);
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Main);

            SetupToolbar();
            SetupNavigation();

            await CreatePresenter(savedInstanceState);
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data?.GetBooleanExtra(SettingsActivity.RecreateActivityIntentKey, false) == true)
            {
                Recreate();
            }
        }

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                _navDrawer.OpenDrawer(GravityCompat.Start);
                return true;
            }

            return false;
        }

        #endregion

        #region Navigation

        private void SetupNavigation()
        {
            var navHeader = _navigationView.GetHeaderView(0);

            var settingsButton = navHeader.FindViewById<ImageButton>(Resource.Id.Navigation_SettingsButton);
            settingsButton.Click += (settingsSender, settingsEventArgs) =>
            {
                _navClosedAction = () => SettingsActivity.StartActivity(this);
                _navDrawer.CloseDrawer(GravityCompat.Start);
            };

            _navDrawer.DrawerClosed += OnDrawerClosed;
        }

        //this is needed to delay fragment replacement until the drawer is actually closed, so as not to show crappy animations
        private void OnDrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            if (_navClosedAction != null)
            {
                var action = _navClosedAction;
                _navClosedAction = null;
                action.Invoke();
            }
        }

        #endregion
    }
}