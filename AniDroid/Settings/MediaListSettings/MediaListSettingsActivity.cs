﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.Main;
using AniDroid.Utils;
using AniDroid.Utils.Comparers;
using Ninject;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Settings.MediaListSettings
{
    [Activity(Label = "Media List Settings")]
    public class MediaListSettingsActivity : BaseAniDroidActivity<MediaListSettingsPresenter>, IMediaListSettingsView
    {
        [InjectView(Resource.Id.Settings_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Settings_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Settings_Container)]
        private LinearLayout _settingsContainer;

        private bool _recreateActivity;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IMediaListSettingsView, MediaListSettingsActivity>(this));

        public override void OnError(IAniListError error)
        {
            // TODO: should this ever matter?
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public static void StartActivityForResult(Activity context)
        {
            var intent = new Intent(context, typeof(MediaListSettingsActivity));
            context.StartActivityForResult(intent, 1);
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            _recreateActivity = Intent.GetBooleanExtra(MainActivity.RecreateActivityIntentKey, false);

            SetContentView(Resource.Layout.Activity_Settings);

            SetupToolbar();

            await CreatePresenter(savedInstanceState);
        }

        public void CreateGroupCompletedSettingItem(bool groupCompleted)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateSwitchSettingRow(this, "Group Completed Items", "Choose whether you'd like to group all completed lists together under one list, regardless of how you have it set on AniList", groupCompleted, true, (sender, args) =>
                    Presenter.SetGroupCompleted(args.IsChecked)));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateMediaListViewTypeSettingItem(MediaListRecyclerAdapter.MediaListItemViewType viewType)
        {
            var options = new List<string> { "Normal", "Compact", "Title Only" };
            _settingsContainer.AddView(
                SettingsActivity.CreateSpinnerSettingRow(this, "Media List View", "Choose how you'd like to show media list items", options, (int)viewType, (sender, args) =>
                {
                    Presenter.SetMediaListViewType((MediaListRecyclerAdapter.MediaListItemViewType)args.Position);

                    if (viewType != (MediaListRecyclerAdapter.MediaListItemViewType)args.Position)
                    {
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }
                }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateHighlightPriorityMediaListItemsItem(bool highlightPriorityItems)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateSwitchSettingRow(this, "Highlight Priority Media List Items",
                    "Choose whether you'd like to show a highlighted background on all media list items that you've marked as Priority",
                    highlightPriorityItems, true,
                    (sender, args) =>
                    {
                        Presenter.SetHighlightPriorityMediaListItems(args.IsChecked);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateAnimeListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getAnimeLists)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateChevronSettingRow(this, "Set Anime List Tab Order", null,
                    (sender, args) =>
                    {
                        MediaListTabOrderDialog.Create(this, getAnimeLists.Invoke(), Presenter.SetAnimeListTabOrder);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateMangaListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getMangaLists)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateChevronSettingRow(this, "Set Manga List Tab Order", null,
                    (sender, args) =>
                    {
                        MediaListTabOrderDialog.Create(this, getMangaLists.Invoke(), Presenter.SetMangaListTabOrder);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateAnimeListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateChevronSettingRow(this, "Set Anime List Sort Type", "Set how you want to sort items on your Anime lists", (sender, args) =>
                    MediaListSortDialog.Create(this, sort, direction, Presenter.SetAnimeListSort)));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateMangaListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateChevronSettingRow(this, "Set Manga List Sort Type", "Set how you want to sort items on your Manga lists", (sender, args) =>
                    MediaListSortDialog.Create(this, sort, direction, Presenter.SetMangaListSort)));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateUseLongClickForEpisodeAddItem(bool useLongClickForEpisodeAdd)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateSwitchSettingRow(this, "Use Long Press When Adding Episodes/Chapters",
                    "When this setting is enabled, tapping the plus icon on your anime/manga lists to add an episode/chapter will require you to long press on the icon instead. Use this if you accidentally add episodes when trying to hit the search button.",
                    useLongClickForEpisodeAdd, true,
                    (sender, args) =>
                    {
                        Presenter.SetUseLongClickForEpisodeAdd(args.IsChecked);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateMediaListProgressDisplayItem(MediaListRecyclerAdapter.MediaListProgressDisplayType mediaListProgressDisplay)
        {
            var options = new List<string> { "Never", "Releasing\nOnly", "Releasing +\n1 Week", "Always" };
            _settingsContainer.AddView(
                SettingsActivity.CreateSpinnerSettingRow(this, "Episode Progress Display",
                    "Choose how you'd like to display episode progress. This will change the title color for your currently watched Anime to show how up-to-date you are.",
                    options, (int) mediaListProgressDisplay, (sender, args) =>
                    {
                        Presenter.SetMediaListProgressDisplay((MediaListRecyclerAdapter.MediaListProgressDisplayType) args.Position);

                        if (mediaListProgressDisplay != (MediaListRecyclerAdapter.MediaListProgressDisplayType) args.Position)
                        {
                            _recreateActivity = true;
                            Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                        }
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        public void CreateUseSwipeToRefreshOnMediaLists(bool useSwipeToRefreshOnMediaLists)
        {
            _settingsContainer.AddView(
                SettingsActivity.CreateSwitchSettingRow(this, "Swipe to Refresh Media Lists",
                    "With this enabled, you can swipe down to refresh the content on your media lists",
                    useSwipeToRefreshOnMediaLists, true,
                    (sender, args) =>
                    {
                        Presenter.SetUseSwipeToRefreshOnMediaLists(args.IsChecked);
                        _recreateActivity = true;
                        Intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
                    }));
            _settingsContainer.AddView(SettingsActivity.CreateSettingDivider(this));
        }

        #region Toolbar

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

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
            }

            return false;
        }

        #endregion

    }
}