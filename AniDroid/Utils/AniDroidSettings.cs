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
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Storage;

namespace AniDroid.Utils
{
    internal class AniDroidSettings : IAniDroidSettings
    {
        private readonly AniDroidStorage _settingStorage;

        public AniDroidSettings(AniDroidStorage settingsStorage)
        {
            _settingStorage = settingsStorage;
        }

        public BaseRecyclerAdapter.CardType CardType
        {
            get => _settingStorage.Get(StorageKeys.CardTypeKey, BaseRecyclerAdapter.CardType.Vertical);
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
            get => _settingStorage.Get(StorageKeys.AccessCode);
            set => _settingStorage.Put(StorageKeys.AccessCode, value);
        }

        public bool IsUserAuthenticated => !string.IsNullOrWhiteSpace(UserAccessCode);

        public void ClearUserAuthentication()
        {
            _settingStorage.Put(StorageKeys.AccessCode, null);
        }

        #region Constants

        private static class StorageKeys
        {
            public const string CardTypeKey = "CARD_TYPE";
            public const string ThemeKey = "THEME";
            public const string DisplayBannersKey = "DISPLAY_BANNERS";

            public const string AccessCode = "ACCESS_CODE";
        }

        #endregion
    }
}