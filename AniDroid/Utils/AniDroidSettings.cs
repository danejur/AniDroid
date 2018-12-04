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

        public int HighestVersionUsed
        {
            get => _settingStorage.Get(StorageKeys.HighestVersionUsed, 0);
            set => _settingStorage.Put(StorageKeys.HighestVersionUsed, value);
        }

        public BaseRecyclerAdapter.RecyclerCardType CardType
        {
            get => _settingStorage.Get(StorageKeys.CardTypeKey, BaseRecyclerAdapter.RecyclerCardType.Vertical);
            set => _settingStorage.Put(StorageKeys.CardTypeKey, value);
        }

        public BaseAniDroidActivity.AniDroidTheme Theme
        {
            get => _settingStorage.Get(StorageKeys.ThemeKey, BaseAniDroidActivity.AniDroidTheme.AniList);
            set => _settingStorage.Put(StorageKeys.ThemeKey, value);
        }

        public bool DisplayBanners
        {
            get => _settingStorage.Get(StorageKeys.DisplayBannersKey, true);
            set => _settingStorage.Put(StorageKeys.DisplayBannersKey, value);
        }

        public string UserAccessCode
        {
            get => _authSettingStorage.Get(StorageKeys.AccessCode);
            set => _authSettingStorage.Put(StorageKeys.AccessCode, value);
        }

        public bool IsUserAuthenticated => !string.IsNullOrWhiteSpace(UserAccessCode);

        public void ClearUserAuthentication()
        {
            UserAccessCode = null;
            LoggedInUser = null;
        }

        public User LoggedInUser
        {
            get => _authSettingStorage.Get<User>(StorageKeys.LoggedInUser);
            set => _authSettingStorage.Put(StorageKeys.LoggedInUser, value);
        }

        public bool ShowAllAniListActivity
        {
            get => _authSettingStorage.Get(StorageKeys.ShowAllActivityKey, false);
            set => _authSettingStorage.Put(StorageKeys.ShowAllActivityKey, value);
        }

        public List<KeyValuePair<string, bool>> AnimeListOrder
        {
            get => _authSettingStorage.Get(StorageKeys.AnimeListOrderKey, (List<KeyValuePair<string, bool>>)null);
            set => _authSettingStorage.Put(StorageKeys.AnimeListOrderKey, value);
        }

        public List<KeyValuePair<string, bool>> MangaListOrder
        {
            get => _authSettingStorage.Get(StorageKeys.MangaListOrderKey, (List<KeyValuePair<string, bool>>)null);
            set => _authSettingStorage.Put(StorageKeys.MangaListOrderKey, value);
        }

        public bool GroupCompletedLists
        {
            get => _authSettingStorage.Get(StorageKeys.GroupCompletedLists, false);
            set => _authSettingStorage.Put(StorageKeys.GroupCompletedLists, value);
        }

        public MediaListRecyclerAdapter.MediaListItemViewType MediaViewType
        {
            get => _authSettingStorage.Get(StorageKeys.MediaViewType, MediaListRecyclerAdapter.MediaListItemViewType.Normal);
            set => _authSettingStorage.Put(StorageKeys.MediaViewType, value);
        }

        public bool HighlightPriorityMediaListItems
        {
            get => _authSettingStorage.Get(StorageKeys.HighlightPriorityMediaListItems, true);
            set => _authSettingStorage.Put(StorageKeys.HighlightPriorityMediaListItems, value);
        }

        public bool DisplayMediaListItemProgressColors
        {
            get => _authSettingStorage.Get(StorageKeys.DisplayMediaListItemProgressColors, true);
            set => _authSettingStorage.Put(StorageKeys.DisplayMediaListItemProgressColors, value);
        }
		
		public MediaListSortComparer.MediaListSortType AnimeListSortType
        {
            get => _authSettingStorage.Get(StorageKeys.AnimeListSortTypeKey, MediaListSortComparer.MediaListSortType.NoSort);
            set => _authSettingStorage.Put(StorageKeys.AnimeListSortTypeKey, value);
        }

        public MediaListSortComparer.MediaListSortDirection AnimeListSortDirection
        {
            get => _authSettingStorage.Get(StorageKeys.AnimeListSortDirectionKey, MediaListSortComparer.MediaListSortDirection.Descending);
            set => _authSettingStorage.Put(StorageKeys.AnimeListSortDirectionKey, value);
        }

        public MediaListSortComparer.MediaListSortType MangaListSortType
        {
            get => _authSettingStorage.Get(StorageKeys.MangaListSortTypeKey, MediaListSortComparer.MediaListSortType.NoSort);
            set => _authSettingStorage.Put(StorageKeys.MangaListSortTypeKey, value);
        }

        public MediaListSortComparer.MediaListSortDirection MangaListSortDirection
        {
            get => _authSettingStorage.Get(StorageKeys.MangaListSortDirectionKey, MediaListSortComparer.MediaListSortDirection.Descending);
            set => _authSettingStorage.Put(StorageKeys.MangaListSortDirectionKey, value);
        }

        public bool UseLongClickForEpisodeAdd
        {
            get => _authSettingStorage.Get(StorageKeys.UseLongClickForEpisodeAddKey, false);
            set => _authSettingStorage.Put(StorageKeys.UseLongClickForEpisodeAddKey, value);
        }

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

        private static class StorageKeys
        {
            public const string HighestVersionUsed = "HIGHEST_VERSION_USED";
            public const string CardTypeKey = "CARD_TYPE";
            public const string ThemeKey = "THEME";
            public const string DisplayBannersKey = "DISPLAY_BANNERS";

            public const string AccessCode = "ACCESS_CODE";
            public const string LoggedInUser = "LOGGED_IN_USER";
            public const string AnimeListOrderKey = "ANIME_LIST_ORDER_KEY";
            public const string MangaListOrderKey = "MANGA_LIST_ORDER_KEY";
            public const string GroupCompletedLists = "GROUP_COMPLETED_LISTS";
            public const string MediaViewType = "MEDIA_VIEW_TYPE";
            public const string HighlightPriorityMediaListItems = "HIGHLIGHT_PRIORITY_MEDIA_LIST_ITEMS";
            public const string DisplayMediaListItemProgressColors = "DISPLAY_MEDIA_LIST_ITEM_PROGRESS_COLORS";
            public const string AnimeListSortTypeKey = "ANIME_LIST_SORT_TYPE";
            public const string AnimeListSortDirectionKey = "ANIME_LIST_SORT_DIRECTION";
            public const string MangaListSortTypeKey = "MANGA_LIST_SORT_TYPE";
            public const string MangaListSortDirectionKey = "MANGA_LIST_SORT_DIRECTION";
            public const string UseLongClickForEpisodeAddKey = "USE_LONG_CLICK_FOR_EPISODE_ADD";

            public const string ShowAllActivityKey = "SHOW_ALL_ACTIVITY";
        }

        #endregion
    }
}