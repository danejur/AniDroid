using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Base;

namespace AniDroid.Settings
{
    public interface ISettingsView : IAniDroidView
    {
        void CreateCardTypeSettingItem(BaseRecyclerAdapter.RecyclerCardType cardType);
        void CreateAniDroidThemeSettingItem(BaseAniDroidActivity.AniDroidTheme theme);
        void CreateDisplayBannersSettingItem(bool displayBanners);
    }
}