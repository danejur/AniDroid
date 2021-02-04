using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.ViewPager.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Utils;
using Google.Android.Material.AppBar;
using Google.Android.Material.Snackbar;
using Google.Android.Material.Tabs;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace AniDroid.Base
{
    public abstract class BaseAniListObjectActivity<T> : BaseAniDroidActivity<T>, IAniListObjectView where T : BaseAniDroidPresenter
    {
        protected const int PageLength = 20;

        private bool _isStandAloneActivity;
        private bool _showMenu;
        private bool _canFavorite;
        private bool _isFavorite;
        private bool _hasBanner;
        private string _shareTitle;
        private string _shareUri;
        private IMenu _menu;

        [InjectView(Resource.Id.AniListObject_Tabs)]
        protected TabLayout TabLayout;
        [InjectView(Resource.Id.AniListObject_ViewPager)]
        protected ViewPager ViewPager;

        protected CoordinatorLayout CoordLayout;
        protected Toolbar Toolbar;
        protected AppBarLayout AppBar;

        protected virtual Func<Task> ToggleFavorite { get; set; }

        public void Share()
        {
            if (string.IsNullOrWhiteSpace(_shareTitle) || string.IsNullOrWhiteSpace(_shareUri))
            {
                DisplaySnackbarMessage("Error occurred while sharing");
                return;
            }

            var extraText = $"{_shareTitle}\n{_shareUri}";
            var sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, extraText);
            sendIntent.SetType("text/plain");
            StartActivity(sendIntent);
        }

        public sealed override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            Snackbar.Make(CoordLayout, message, length).Show();
        }

        public sealed override void OnError(IAniListError error)
        {
            var title = "Error!";
            var message = "";

            if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
            {
                title = "Something happened while processing your request!";
                message = error.ErrorMessage;
            }
            else if (error.ErrorException != null)
            {
                title = "An exception occurred while processing your request!";
                message = error.ErrorException.Message;
            }
            else if (error.GraphQLErrors?.Any() == true)
            {
                title = "A GraphQL related error occurred while processing your request!";
                message = error.GraphQLErrors.First().Message ?? "";
            }


            SetErrorShown(title, message);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == ObjectBrowseRequestCode && resultCode == Result.Ok)
            {
                SetResult(Result.Ok);
                Finish();
            }
        }

        #region View Implementation

        public void SetLoadingShown()
        {
            SetContentView(Resource.Layout.Activity_Loading);
            CoordLayout = FindViewById<CoordinatorLayout>(Resource.Id.Loading_CoordLayout);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Loading_Toolbar);
        }

        public void SetContentShown(bool hasBanner = false)
        {
            _hasBanner = hasBanner && Settings.DisplayBanners;
            ShowMenu();
            SetContentView(_hasBanner ? Resource.Layout.Activity_AniListObject_Banner : Resource.Layout.Activity_AniListObject);
            CoordLayout = FindViewById<CoordinatorLayout>(Resource.Id.AniListObject_CoordLayout);
            Toolbar = FindViewById<Toolbar>(Resource.Id.AniListObject_Toolbar);
            AppBar = FindViewById<AppBarLayout>(Resource.Id.AniListObject_AppBar);
        }

        public void SetErrorShown(string title, string message)
        {
            SetContentView(Resource.Layout.View_Error);
            CoordLayout = FindViewById<CoordinatorLayout>(Resource.Id.Error_CoordLayout);

            FindViewById<TextView>(Resource.Id.Error_Title).Text = title;
            FindViewById<TextView>(Resource.Id.Error_Message).Text = message;

            var toolbar = FindViewById<Toolbar>(Resource.Id.Error_Toolbar);
            ((AppBarLayout.LayoutParams)toolbar.LayoutParameters).ScrollFlags = 0;
            toolbar.Title = "Error";
            SetSupportActionBar(toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            HasError = true;
            InvalidateOptionsMenu();
        }

        public void SetIsFavorite(bool isFavorite, bool showNotification = false)
        {
            _canFavorite = Presenter.AniDroidSettings.IsUserAuthenticated;
            _isFavorite = isFavorite;
            _menu?.FindItem(Resource.Id.Menu_AniListObject_Favorite)?.SetIcon(_isFavorite
                ? Resource.Drawable.ic_favorite_white_24px
                : Resource.Drawable.ic_favorite_border_white_24px);

            if (showNotification)
            {
                DisplaySnackbarMessage("Favorite toggled");
            }
        }

        public void SetShareText(string title, string uri)
        {
            _shareTitle = title;
            _shareUri = uri;
        }

        public void SetupToolbar(string text, string bannerUri = null)
        {
            Toolbar.Title = text;
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (!_hasBanner)
            {
                ((AppBarLayout.LayoutParams)Toolbar.LayoutParameters).ScrollFlags = 0;
            }
            else if (!string.IsNullOrWhiteSpace(bannerUri))
            {
                var bannerView = FindViewById<ImageView>(Resource.Id.AniListObject_BannerImage);
                ImageLoader.LoadImage(bannerView, bannerUri, false);
            }
        }

        public void SetStandaloneActivity()
        {
            _isStandAloneActivity = true;
        }

        #endregion

        #region Toolbar

        protected void ShowMenu()
        {
            _showMenu = true;
        }

        protected void SetupAniListObjectToolbar(string title)
        {
            ((AppBarLayout.LayoutParams)Toolbar.LayoutParameters).ScrollFlags = 0;
            Toolbar.Title = title;
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
        

        public override bool SetupMenu(IMenu menu)
        {
            menu?.Clear();
            MenuInflater.Inflate(Resource.Menu.AniListObject_ActionBar, _menu = menu);
            menu?.FindItem(Resource.Id.Menu_AniListObject_Share)?.SetVisible(_showMenu);
            menu?.FindItem(Resource.Id.Menu_AniListObject_Favorite)?.SetVisible(_showMenu && _canFavorite);
            menu?.FindItem(Resource.Id.Menu_AniListObject_Favorite)?.SetIcon(_isFavorite
                ? Resource.Drawable.ic_favorite_white_24px
                : Resource.Drawable.ic_favorite_border_white_24px);
            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Ok);
                    Finish();
                    if (_isStandAloneActivity)
                    {
                        // TODO: implement
                        //GoBackToMainActivity(this);
                    }
                    return true;
                case Resource.Id.Menu_AniListObject_Favorite:
                    ToggleFavorite?.Invoke();
                    return true;
                case Resource.Id.Menu_AniListObject_Share:
                    Share();
                    return true;
            }

            return false;
        }

        #endregion

    }
}