using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils.Comparers;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Storage;

namespace AniDroid.Utils
{
    internal class AniDroidSettings : IAniDroidSettings
    {
        private readonly SettingsStorage _settingStorage;
        private readonly AuthSettingsStorage _authSettingStorage;

        public AniDroidSettings(SettingsStorage settingsStorage, AuthSettingsStorage authSettingsStorage)
        {
            _settingStorage = settingsStorage;
            _authSettingStorage = authSettingsStorage;
        }

        #region Unauthenticated Settings

        public int HighestVersionUsed
        {
            get => _settingStorage.Get(UnauthenticatedKeys.HighestVersionUsed, 0);
            set => _settingStorage.Put(UnauthenticatedKeys.HighestVersionUsed, value);
        }

        public BaseRecyclerAdapter.RecyclerCardType CardType
        {
            get => _settingStorage.Get(UnauthenticatedKeys.CardTypeKey, BaseRecyclerAdapter.RecyclerCardType.Vertical);
            set => _settingStorage.Put(UnauthenticatedKeys.CardTypeKey, value);
        }

        public BaseAniDroidActivity.AniDroidTheme Theme
        {
            get => _settingStorage.Get(UnauthenticatedKeys.ThemeKey, BaseAniDroidActivity.AniDroidTheme.AniList);
            set => _settingStorage.Put(UnauthenticatedKeys.ThemeKey, value);
        }

        public bool DisplayBanners
        {
            get => _settingStorage.Get(UnauthenticatedKeys.DisplayBannersKey, true);
            set => _settingStorage.Put(UnauthenticatedKeys.DisplayBannersKey, value);
        }

        public bool DisplayUpcomingEpisodeTimeAsCountdown
        {
            get => _authSettingStorage.Get(UnauthenticatedKeys.DisplayUpcomingEpisodeTimeAsCountdownKey, false);
            set => _authSettingStorage.Put(UnauthenticatedKeys.DisplayUpcomingEpisodeTimeAsCountdownKey, value);
        }

        public bool UseSwipeToRefreshHomeScreen
        {
            get => _authSettingStorage.Get(UnauthenticatedKeys.UseSwipeToRefreshHomeScreenKey, false);
            set => _authSettingStorage.Put(UnauthenticatedKeys.UseSwipeToRefreshHomeScreenKey, value);
        }

        #endregion

        #region Authenticated Settings

        public string UserAccessCode
        {
            get => _authSettingStorage.Get(AuthenticatedKeys.AccessCode);
            set => _authSettingStorage.Put(AuthenticatedKeys.AccessCode, value);
        }

        public bool IsUserAuthenticated => !string.IsNullOrWhiteSpace(UserAccessCode);

        public void ClearUserAuthentication()
        {
            UserAccessCode = null;
            LoggedInUser = null;
        }

        public User LoggedInUser
        {
            get => _authSettingStorage.Get<User>(AuthenticatedKeys.LoggedInUser);
            set => _authSettingStorage.Put(AuthenticatedKeys.LoggedInUser, value);
        }

        public bool ShowAllAniListActivity
        {
            get => _authSettingStorage.Get(AuthenticatedKeys.ShowAllActivityKey, false);
            set => _authSettingStorage.Put(AuthenticatedKeys.ShowAllActivityKey, value);
        }

        public bool EnableNotificationService
        {
            get => _authSettingStorage.Get(AuthenticatedKeys.EnableNotificationServiceKey, true);
            set => _authSettingStorage.Put(AuthenticatedKeys.EnableNotificationServiceKey, value);
        }

        public MainActivity.DefaultTab DefaultTab
        {
            get => _authSettingStorage.Get(AuthenticatedKeys.DefaultTabKey, MainActivity.DefaultTab.Anime);
            set => _authSettingStorage.Put(AuthenticatedKeys.DefaultTabKey, value);
        }

        #endregion

        #region Media List Settings

        public List<KeyValuePair<string, bool>> AnimeListOrder
        {
            get => _authSettingStorage.Get(MediaListKeys.AnimeListOrderKey, (List<KeyValuePair<string, bool>>)null);
            set => _authSettingStorage.Put(MediaListKeys.AnimeListOrderKey, value);
        }

        public List<KeyValuePair<string, bool>> MangaListOrder
        {
            get => _authSettingStorage.Get(MediaListKeys.MangaListOrderKey, (List<KeyValuePair<string, bool>>)null);
            set => _authSettingStorage.Put(MediaListKeys.MangaListOrderKey, value);
        }

        public bool GroupCompletedLists
        {
            get => _authSettingStorage.Get(MediaListKeys.GroupCompletedLists, false);
            set => _authSettingStorage.Put(MediaListKeys.GroupCompletedLists, value);
        }

        public MediaListRecyclerAdapter.MediaListItemViewType MediaViewType
        {
            get => _authSettingStorage.Get(MediaListKeys.MediaViewType, MediaListRecyclerAdapter.MediaListItemViewType.Normal);
            set => _authSettingStorage.Put(MediaListKeys.MediaViewType, value);
        }

        public bool HighlightPriorityMediaListItems
        {
            get => _authSettingStorage.Get(MediaListKeys.HighlightPriorityMediaListItems, true);
            set => _authSettingStorage.Put(MediaListKeys.HighlightPriorityMediaListItems, value);
        }

        public MediaListSortComparer.MediaListSortType AnimeListSortType
        {
            get => _authSettingStorage.Get(MediaListKeys.AnimeListSortTypeKey, MediaListSortComparer.MediaListSortType.NoSort);
            set => _authSettingStorage.Put(MediaListKeys.AnimeListSortTypeKey, value);
        }

        public MediaListSortComparer.MediaListSortDirection AnimeListSortDirection
        {
            get => _authSettingStorage.Get(MediaListKeys.AnimeListSortDirectionKey, MediaListSortComparer.MediaListSortDirection.Descending);
            set => _authSettingStorage.Put(MediaListKeys.AnimeListSortDirectionKey, value);
        }

        public MediaListSortComparer.MediaListSortType MangaListSortType
        {
            get => _authSettingStorage.Get(MediaListKeys.MangaListSortTypeKey, MediaListSortComparer.MediaListSortType.NoSort);
            set => _authSettingStorage.Put(MediaListKeys.MangaListSortTypeKey, value);
        }

        public MediaListSortComparer.MediaListSortDirection MangaListSortDirection
        {
            get => _authSettingStorage.Get(MediaListKeys.MangaListSortDirectionKey, MediaListSortComparer.MediaListSortDirection.Descending);
            set => _authSettingStorage.Put(MediaListKeys.MangaListSortDirectionKey, value);
        }

        public bool UseLongClickForEpisodeAdd
        {
            get => _authSettingStorage.Get(MediaListKeys.UseLongClickForEpisodeAddKey, false);
            set => _authSettingStorage.Put(MediaListKeys.UseLongClickForEpisodeAddKey, value);
        }

        public MediaListRecyclerAdapter.MediaListProgressDisplayType MediaListProgressDisplay
        {
            get => _authSettingStorage.Get(MediaListKeys.MediaListProgressDisplayKey,
                _authSettingStorage.Get(OldSettingsStorageKeys.DisplayMediaListItemProgressColors, true)
                    ? MediaListRecyclerAdapter.MediaListProgressDisplayType.Releasing
                    : MediaListRecyclerAdapter.MediaListProgressDisplayType.Never);
            set => _authSettingStorage.Put(MediaListKeys.MediaListProgressDisplayKey, value);
        }

        public bool UseSwipeToRefreshOnMediaLists
        {
            get => _authSettingStorage.Get(MediaListKeys.UseSwipeToRefreshMediaListsKey, false);
            set => _authSettingStorage.Put(MediaListKeys.UseSwipeToRefreshMediaListsKey, value);
        }

        public bool ShowEpisodeAddButtonForRepeatingMedia
        {
            get => _authSettingStorage.Get(MediaListKeys.ShowEpisodeAddButtonForRepeatingMediaKey, false);
            set => _authSettingStorage.Put(MediaListKeys.ShowEpisodeAddButtonForRepeatingMediaKey, value);
        }

        #endregion

        #region Old Settings

        public bool DisplayMediaListItemProgressColors
        {
            get => _authSettingStorage.Get(OldSettingsStorageKeys.DisplayMediaListItemProgressColors, true);
            set => _authSettingStorage.Put(OldSettingsStorageKeys.DisplayMediaListItemProgressColors, value);
        }

        public bool AlwaysDisplayEpisodeProgressColor
        {
            get => _authSettingStorage.Get(OldSettingsStorageKeys.AlwaysDisplayEpisodeProgressColorKey, false);
            set => _authSettingStorage.Put(OldSettingsStorageKeys.AlwaysDisplayEpisodeProgressColorKey, value);
        }

        #endregion

        #region Methods

        public void UpdateLoggedInUser(User user)
        {
            LoggedInUser = user;
            UpdateUserMediaListTabs(user.MediaListOptions);
        }

        public void UpdateUserMediaListTabs(User.UserMediaListOptions mediaListOptions)
        {
            if (AnimeListOrder == null)
            {
                AnimeListOrder = new List<KeyValuePair<string, bool>>();
            }

            var animeLists = AnimeListOrder.ToList();
            animeLists.RemoveAll(x => !mediaListOptions.AnimeList.SectionOrder.Contains(x.Key));

            // add any missing lists with the default value of visible
            animeLists.AddRange(mediaListOptions.AnimeList.SectionOrder
                .Where(y => !animeLists.Select(z => z.Key).Contains(y))
                .Select(x => new KeyValuePair<string, bool>(x, true)));
            AnimeListOrder = animeLists;

            if (MangaListOrder == null)
            {
                MangaListOrder = new List<KeyValuePair<string, bool>>();
            }

            var mangaLists = MangaListOrder.ToList();
            mangaLists.RemoveAll(x => !mediaListOptions.MangaList.SectionOrder.Contains(x.Key));

            mangaLists.AddRange(mediaListOptions.MangaList.SectionOrder.Where(y => !mangaLists.Select(z => z.Key).Contains(y))
                .Select(x => new KeyValuePair<string, bool>(x, true)));
            MangaListOrder = mangaLists;
        }

        #endregion

        #region Constants

        private static class UnauthenticatedKeys
        {
            public const string HighestVersionUsed = "HIGHEST_VERSION_USED";
            public const string CardTypeKey = "CARD_TYPE";
            public const string ThemeKey = "THEME";
            public const string DisplayBannersKey = "DISPLAY_BANNERS";
            public const string UseSwipeToRefreshHomeScreenKey = "USE_SWIPE_TO_REFRESH_HOME_SCREEN";
            public const string DisplayUpcomingEpisodeTimeAsCountdownKey = "DISPLAY_UPCOMING_EPISODE_TIME_AS_COUNTDOWN";
        }

        private static class AuthenticatedKeys
        {
            public const string AccessCode = "ACCESS_CODE";
            public const string LoggedInUser = "LOGGED_IN_USER";
            public const string ShowAllActivityKey = "SHOW_ALL_ACTIVITY";
            public const string EnableNotificationServiceKey = "ENABLE_NOTIFICATION_SERVICE";
            public const string DefaultTabKey = "DEFAULT_TAB";
        }

        private static class MediaListKeys
        {
            public const string AnimeListOrderKey = "ANIME_LIST_ORDER_KEY";
            public const string MangaListOrderKey = "MANGA_LIST_ORDER_KEY";
            public const string GroupCompletedLists = "GROUP_COMPLETED_LISTS";
            public const string MediaViewType = "MEDIA_VIEW_TYPE";
            public const string HighlightPriorityMediaListItems = "HIGHLIGHT_PRIORITY_MEDIA_LIST_ITEMS";
            public const string AnimeListSortTypeKey = "ANIME_LIST_SORT_TYPE";
            public const string AnimeListSortDirectionKey = "ANIME_LIST_SORT_DIRECTION";
            public const string MangaListSortTypeKey = "MANGA_LIST_SORT_TYPE";
            public const string MangaListSortDirectionKey = "MANGA_LIST_SORT_DIRECTION";
            public const string UseLongClickForEpisodeAddKey = "USE_LONG_CLICK_FOR_EPISODE_ADD";
            public const string MediaListProgressDisplayKey = "MEDIA_LIST_PROGRESS_DISPLAY";
            public const string UseSwipeToRefreshMediaListsKey = "USE_SWIPE_TO_REFRESH_MEDIA_LISTS";
            public const string ShowEpisodeAddButtonForRepeatingMediaKey = "SHOW_EPISODE_ADD_BUTTON_FOR_REPEATING_Media";
        }

        private static class OldSettingsStorageKeys
        {
            public const string DisplayMediaListItemProgressColors = "DISPLAY_MEDIA_LIST_ITEM_PROGRESS_COLORS";
            public const string AlwaysDisplayEpisodeProgressColorKey = "ALWAYS_DISPLAY_EPISODE_PROGRESS_COLOR";
        }

        #endregion
    }
}