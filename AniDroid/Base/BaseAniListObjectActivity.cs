using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using AniDroid.Utils;

namespace AniDroid.Base
{
    public abstract class BaseAniListObjectActivity<T> : BaseAniDroidActivity<T>, IAniListObjectView where T : BaseAniDroidPresenter
    {
        protected const int PageLength = 20;

        private bool _isStandAloneActivity;
        private bool _showMenu;
        private bool _isFavorite;
        private string _shareTitle;
        private string _shareUri;
        private IMenu _menu;

        [InjectView(Resource.Id.AniListObject_Tabs)]
        protected TabLayout TabLayout;
        [InjectView(Resource.Id.AniListObject_ViewPager)]
        protected ViewPager ViewPager;

        protected CoordinatorLayout CoordLayout;
        protected Toolbar Toolbar;

        protected virtual Action ToggleFavorite { get; set; }

        public void Share()
        {
            if (string.IsNullOrWhiteSpace(_shareTitle) || string.IsNullOrWhiteSpace(_shareUri))
            {
                DisplaySnackbarMessage("Error occurred while sharing", Snackbar.LengthShort);
                return;
            }

            var extraText = $"{_shareTitle}\n{_shareUri}";
            var sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, extraText);
            sendIntent.SetType("text/plain");
            StartActivity(sendIntent);
        }

        public sealed override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(CoordLayout, message, length).Show();
        }

        public sealed override void OnNetworkError()
        {

        }

        #region View Implementation

        public void SetLoadingShown()
        {
            SetContentView(Resource.Layout.Activity_Loading);
            CoordLayout = FindViewById<CoordinatorLayout>(Resource.Id.Loading_CoordLayout);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Loading_Toolbar);
        }

        public void SetContentShown()
        {
            ShowMenu();
            SetContentView(Resource.Layout.Activity_AniListObject);
            CoordLayout = FindViewById<CoordinatorLayout>(Resource.Id.AniListObject_CoordLayout);
            Toolbar = FindViewById<Toolbar>(Resource.Id.AniListObject_Toolbar);
        }

        public void SetIsFavorite(bool isFavorite)
        {
            _isFavorite = isFavorite;
            _menu?.FindItem(Resource.Id.Menu_AniListObject_Favorite)?.SetIcon(_isFavorite
                ? Resource.Drawable.ic_favorite_white_24dp
                : Resource.Drawable.ic_favorite_border_white_24dp);
        }

        public void SetShareText(string title, string uri)
        {
            _shareTitle = title;
            _shareUri = uri;
        }

        public void SetupToolbar(string text)
        {
            ((AppBarLayout.LayoutParams)Toolbar.LayoutParameters).ScrollFlags = 0;
            Toolbar.Title = text;
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
        

        public sealed override bool SetupMenu(IMenu menu)
        {
            menu?.Clear();
            MenuInflater.Inflate(Resource.Menu.AniListObject_ActionBar, _menu = menu);
            menu?.FindItem(Resource.Id.Menu_AniListObject_Share)?.SetVisible(_showMenu);
            menu?.FindItem(Resource.Id.Menu_AniListObject_Favorite)?.SetVisible(_showMenu);
            SetIsFavorite(_isFavorite);
            return true;
        }

        public sealed override bool MenuItemSelected(IMenuItem item)
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