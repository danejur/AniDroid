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

        public void SetCardType(BaseRecyclerAdapter.CardType cardType)
        {
            _settingStorage.Put(StorageKeys.CardTypeKey, cardType);
        }

        public async Task<BaseRecyclerAdapter.CardType> GetCardTypeAsync()
        {
            return await _settingStorage.Get(StorageKeys.CardTypeKey, BaseRecyclerAdapter.CardType.Vertical);
        }

        public void SetTheme(BaseAniDroidActivity.AniDroidTheme theme)
        {
            _settingStorage.Put(StorageKeys.ThemeKey, theme);
        }

        public async Task<BaseAniDroidActivity.AniDroidTheme> GetThemeAsync()
        {
            return await _settingStorage.Get(StorageKeys.ThemeKey, BaseAniDroidActivity.AniDroidTheme.AniList);
        }

        #region Constants

        private static class StorageKeys
        {
            public const string CardTypeKey = "CARD_TYPE";
            public const string ThemeKey = "THEME";
        }

        #endregion
    }
}