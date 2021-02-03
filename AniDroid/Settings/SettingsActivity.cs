using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Work;
using AniDroid.About;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Jobs;
using AniDroid.Main;
using AniDroid.Settings.MediaListSettings;
using AniDroid.Utils;
using Google.Android.Material.Snackbar;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace AniDroid.Settings
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : BaseAniDroidActivity<SettingsPresenter>, ISettingsView
    {
        [InjectView(Resource.Id.Settings_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Settings_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Settings_Container)]
        private LinearLayout _settingsContainer;

        private bool _recreateActivity;

        public override void OnError(IAniListError error)
        {
            // TODO: should this ever matter?
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public void CreateCardTypeSettingItem(BaseRecyclerAdapter.RecyclerCardType cardType)
        {
            var options = new List<string> {"Vertical", "Horizontal", "Flat Horizontal"};
            _settingsContainer.AddView(
                CreateSpinnerSettingRow(this, "Card Display Type", "Choose how you would like to display lists in AniDroid",
                    options, (int) cardType,
                    (sender, args) =>
                    {
                        Presenter.SetCardType((BaseRecyclerAdapter.RecyclerCardType) args.Position);

                        if (cardType != (BaseRecyclerAdapter.RecyclerCardType) args.Position)
                        {
                            _recreateActivity = true;
                            Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                        }
                    }));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateAniDroidThemeSettingItem(AniDroidTheme theme)
        {
            var options = new List<string> { "AniList", "Black", "AniList Dark", "Dark" };
            _settingsContainer.AddView(
                CreateSpinnerSettingRow(this, "AniDroid Theme", "Choose the theme you'd like to use", options, (int)theme, (sender, args) =>
                {
                    Presenter.SetTheme((AniDroidTheme) args.Position);

                    if (theme != (AniDroidTheme) args.Position)
                    {
                        Recreate();
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }
                }));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateDisplayBannersSettingItem(bool displayBanners)
        {
            _settingsContainer.AddView(
                CreateSwitchSettingRow(this, "Display Banners", "Choose whether you'd like to display banner images for Media and Users", displayBanners, true, (sender, args) =>
                    Presenter.SetDisplayBanners(args.IsChecked)));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateDisplayUpcomingEpisodeTimeAsCountdownItem(bool displayUpcomingEpisodeTimeAsCountdown)
        {
            _settingsContainer.AddView(
                CreateSwitchSettingRow(this, "Display Upcoming Episode Times As Countdowns",
                    "Upcoming episode times will appear as days and hours remaining instead of the date",
                    displayUpcomingEpisodeTimeAsCountdown, true,
                    (sender, args) =>
                    {
                        Presenter.SetDisplayUpcomingEpisodeTimeAsCountdown(args.IsChecked);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
                        
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateUseSwipeToRefreshHomeScreen(bool useSwipeToRefreshHomeScreen)
        {
            _settingsContainer.AddView(
                CreateSwitchSettingRow(this, "Swipe to Refresh Home Screen",
                    "With this enabled, you can swipe down to refresh the content on the Home screen",
                    useSwipeToRefreshHomeScreen, true, (sender, args) =>
                    {
                        Presenter.SetUseSwipeToRefreshHomeScreen(args.IsChecked);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateWhatsNewSettingItem()
        {
            _settingsContainer.AddView(
                CreateSettingRow(this, "Show What's New", null, (sender, args) =>
                    WhatsNewDialog.Create(this)));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreatePrivacyPolicyLinkItem()
        {
            _settingsContainer.AddView(
                CreateSettingRow(this, "Privacy Policy", null, (sender, args) =>
                {
                    var privacyPolicyUrl = Resources.GetString(Resource.String.AppPrivacyPolicyUrl);
                    var intent = new Intent(Intent.ActionView);
                    intent.SetData(Android.Net.Uri.Parse(privacyPolicyUrl));
                    StartActivity(intent);
                }));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateAboutSettingItem()
        {
            _settingsContainer.AddView(
                CreateSettingRow(this, "About", null, (sender, args) =>
                    AboutActivity.StartActivity(this)));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        #region Auth Settings

        public void CreateMediaListSettingsItem()
        {
            _settingsContainer.AddView(
                CreateChevronSettingRow(this, "Media List Settings", null,
                    (sender, args) => MediaListSettingsActivity.StartActivityForResult(this)));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateEnableNotificationServiceItem(bool enableNotificationService)
        {
            _settingsContainer.AddView(
                CreateSwitchSettingRow(this, "Enable Notification Alerts",
                    "Check for new AniList notifications every 30 minutes.",
                    enableNotificationService, true,
                    (sender, args) =>
                    {
                        Presenter.SetEnableNotificationService(args.IsChecked);

                        if (args.IsChecked)
                        {
                            AniDroidJobManager.EnableAniListNotificationJob(this);
                        }
                        else
                        {
                            AniDroidJobManager.DisableAniListNotificationJob(this);
                        }
                    }));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        public void CreateDefaultTabItem(MainActivity.DefaultTab defaultTab)
        {
            var tabs = Enum.GetNames(typeof(MainActivity.DefaultTab)).ToList();

            _settingsContainer.AddView(
                CreateSpinnerSettingRow(this, "Default Tab",
                    "Choose which tab you'd like to show by default when opening AniDroid", tabs,
                    tabs.IndexOf(defaultTab.ToString()),
                    (sender, args) => Presenter.SetDefaultTab((MainActivity.DefaultTab) args.Position)));
            _settingsContainer.AddView(CreateSettingDivider(this));
        }

        #endregion

        public static void StartActivity(Activity context)
        {
            var intent = new Intent(context, typeof(SettingsActivity));
            context.StartActivityForResult(intent, 1);
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            _recreateActivity = Intent.GetBooleanExtra(MainActivity.RecreateActivityIntentKey, false);

            SetContentView(Resource.Layout.Activity_Settings);

            SetupToolbar();

            await CreatePresenter(savedInstanceState);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data?.GetBooleanExtra(MainActivity.RecreateActivityIntentKey, false) == true)
            {
                _recreateActivity = true;
                Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
            }
        }

        #region Settings Views

        public static View CreateSettingRow(BaseAniDroidActivity context, string name, string description, EventHandler tapEvent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem, null);
            var nameView = view.FindViewById<TextView>(Resource.Id.SettingItem_Name);
            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);

            if (!string.IsNullOrWhiteSpace(name))
            {
                nameView.Text = name;
            }
            else
            {
                nameView.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                textTwoView.Text = description;
            }
            else
            {
                textTwoView.Visibility = ViewStates.Gone;
            }

            view.Id = (int)DateTime.Now.Ticks;
            view.Click += tapEvent;

            return view;
        }

        public static View CreateSwitchSettingRow(BaseAniDroidActivity context, string name, string description, bool switchState, bool useUniqueControlId, EventHandler<CompoundButton.CheckedChangeEventArgs> switchEvent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Switch, null);
            var nameView = view.FindViewById<TextView>(Resource.Id.SettingItem_Name);
            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);

            if (!string.IsNullOrWhiteSpace(name))
            {
                nameView.Text = name;
            }
            else
            {
                nameView.Visibility = ViewStates.Gone;
                textTwoView.SetTextColor(new Color(context.GetThemedColor(Resource.Attribute.Background_Text)));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                textTwoView.Text = description;
            }
            else
            {
                textTwoView.Visibility = ViewStates.Gone;
            }

            var switchView = view.FindViewById<SwitchCompat>(Resource.Id.SettingItem_Switch);

            if (useUniqueControlId)
            {
                switchView.Id = (int) DateTime.Now.Ticks;
            }

            switchView.Checked = switchState;
            switchView.CheckedChange += switchEvent;

            return view;
        }

        public static View CreateCheckboxSettingRow(BaseAniDroidActivity context, string name, string description, bool isChecked, EventHandler<CompoundButton.CheckedChangeEventArgs> checkEvent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Checkbox, null);

            var nameView = view.FindViewById<TextView>(Resource.Id.SettingItem_Name);
            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);

            if (!string.IsNullOrWhiteSpace(name))
            {
                nameView.Text = name;
            }
            else
            {
                nameView.Visibility = ViewStates.Gone;
                textTwoView.SetTextColor(new Color(context.GetThemedColor(Resource.Attribute.Background_Text)));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                textTwoView.Text = description;
            }
            else
            {
                textTwoView.Visibility = ViewStates.Gone;
            }

            var checkboxView = view.FindViewById<AppCompatCheckBox>(Resource.Id.SettingItem_Checkbox);
            checkboxView.Id = (int)DateTime.Now.Ticks;
            checkboxView.Checked = isChecked;
            checkboxView.CheckedChange += checkEvent;

            return view;
        }

        public static View CreateSpinnerSettingRow(BaseAniDroidActivity context, string name, string description, IList<string> items, int selectedPosition, EventHandler<AdapterView.ItemSelectedEventArgs> selectedEvent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Spinner, null);

            var nameView = view.FindViewById<TextView>(Resource.Id.SettingItem_Name);
            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);

            if (!string.IsNullOrWhiteSpace(name))
            {
                nameView.Text = name;
            }
            else
            {
                nameView.Visibility = ViewStates.Gone;
                textTwoView.SetTextColor(new Color(context.GetThemedColor(Resource.Attribute.Background_Text)));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                textTwoView.Text = description;
            }
            else
            {
                textTwoView.Visibility = ViewStates.Gone;
            }

            var spinner = view.FindViewById<Spinner>(Resource.Id.SettingItem_Spinner);
            spinner.Id = (int)DateTime.Now.Ticks;
            spinner.Adapter = new ArrayAdapter<string>(context, Resource.Layout.View_SpinnerDropDownItem, items);
            spinner.SetSelection(selectedPosition);
            spinner.ItemSelected -= selectedEvent;
            spinner.ItemSelected += selectedEvent;

            return view;
        }

        public static View CreateChevronSettingRow(BaseAniDroidActivity context, string name, string description, EventHandler tapEvent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Chevron, null);
            var nameView = view.FindViewById<TextView>(Resource.Id.SettingItem_Name);
            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);

            if (!string.IsNullOrWhiteSpace(name))
            {
                nameView.Text = name;
            }
            else
            {
                nameView.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                textTwoView.Text = description;
            }
            else
            {
                textTwoView.Visibility = ViewStates.Gone;
            }

            view.Id = (int)DateTime.Now.Ticks;
            view.Click += tapEvent;

            return view;
        }

        public static View CreateSettingDivider(BaseAniDroidActivity context)
        {
            var typedValue = new TypedValue();
            context.Theme.ResolveAttribute(Resource.Attribute.ListItem_Divider, typedValue, true);
            var dividerView = new LinearLayout(context) { LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 2) };
            dividerView.SetBackgroundResource(typedValue.ResourceId);

            return dividerView;
        }

        #endregion

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool SetupMenu(IMenu menu)
        {
            menu?.Clear();
            MenuInflater.Inflate(Resource.Menu.Settings_ActionBar, menu);
            menu?.FindItem(Resource.Id.Menu_Settings_Logout)?.SetVisible(Settings.IsUserAuthenticated);
            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (_recreateActivity)
                    {
                        var resultIntent = new Intent();
                        resultIntent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                        SetResult(Result.Canceled, resultIntent);
                    }
                    Finish();
                    return true;
                case Resource.Id.Menu_Settings_Logout:
                    DisplayLogoutDialog();
                    return true;
            }

            return false;
        }

        public override void OnBackPressed()
        {
            if (_recreateActivity)
            {
                var resultIntent = new Intent();
                resultIntent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                SetResult(Result.Canceled, resultIntent);
                Finish();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void DisplayLogoutDialog()
        {
            var alert = new AlertDialog.Builder(this,
                    GetThemedResourceId(Resource.Attribute.Dialog_Theme))
                .SetMessage(Resource.String.LoginLogout_LogoutDialogMessage)
                .SetCancelable(true).Create();
            alert.SetButton((int)DialogButtonType.Neutral, "Cancel", (sender, eventArgs) => alert.Dismiss());
            alert.SetButton((int)DialogButtonType.Positive, "Logout", (sender, eventArgs) =>
            {
                Settings.ClearUserAuthentication();
                var resultIntent = new Intent();
                resultIntent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                SetResult(Result.Canceled, resultIntent);
                Finish();
            });
            alert.Show();
        }

        #endregion

    }
}