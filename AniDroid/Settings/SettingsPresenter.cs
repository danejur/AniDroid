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
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Settings
{
    public class SettingsPresenter : BaseAniDroidPresenter<ISettingsView>
    {
        public SettingsPresenter(ISettingsView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            View.CreateCardTypeSettingItem(AniDroidSettings.CardType);
            View.CreateAniDroidThemeSettingItem(AniDroidSettings.Theme);
            View.CreateDisplayBannersSettingItem(AniDroidSettings.DisplayBanners);
            return Task.CompletedTask;
        }

        public override async Task RestoreState(IList<string> savedState)
        {
            await Init();
        }

        public void SetCardType(BaseRecyclerAdapter.CardType cardType)
        {
            AniDroidSettings.CardType = cardType;
        }

        public void SetTheme(BaseAniDroidActivity.AniDroidTheme theme)
        {
            AniDroidSettings.Theme = theme;
        }

        public void SetDisplayBanners(bool displayBanners)
        {
            AniDroidSettings.DisplayBanners = displayBanners;
        }
    }
}