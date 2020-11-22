using System.Collections.Generic;
using System.Threading.Tasks;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.Settings
{
    public class SettingsPresenter : BaseAniDroidPresenter<ISettingsView>
    {
        public SettingsPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            View.CreateCardTypeSettingItem(AniDroidSettings.CardType);
            View.CreateAniDroidThemeSettingItem(AniDroidSettings.Theme);
            View.CreateDisplayBannersSettingItem(AniDroidSettings.DisplayBanners);
            View.CreateDisplayUpcomingEpisodeTimeAsCountdownItem(AniDroidSettings
                .DisplayUpcomingEpisodeTimeAsCountdown);
            View.CreateUseSwipeToRefreshHomeScreen(AniDroidSettings.UseSwipeToRefreshHomeScreen);

            if (AniDroidSettings.IsUserAuthenticated)
            {
                View.CreateMediaListSettingsItem();
                View.CreateEnableNotificationServiceItem(AniDroidSettings.EnableNotificationService);
                View.CreateDefaultTabItem(AniDroidSettings.DefaultTab);
            }

            View.CreatePrivacyPolicyLinkItem();
            View.CreateWhatsNewSettingItem();
            View.CreateAboutSettingItem();

            return Task.CompletedTask;
        }

        public override async Task RestoreState(IList<string> savedState)
        {
            await Init();
        }

        public void SetCardType(BaseRecyclerAdapter.RecyclerCardType cardType)
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

        public void SetDisplayUpcomingEpisodeTimeAsCountdown(bool displayUpcomingEpisodeTimeAsCountdown)
        {
            AniDroidSettings.DisplayUpcomingEpisodeTimeAsCountdown = displayUpcomingEpisodeTimeAsCountdown;
        }

        public void SetUseSwipeToRefreshHomeScreen(bool useSwipeToRefreshHomeScreen)
        {
            AniDroidSettings.UseSwipeToRefreshHomeScreen = useSwipeToRefreshHomeScreen;
        }

        // Auth Settings

        public void SetEnableNotificationService(bool enableNotificationService)
        {
            AniDroidSettings.EnableNotificationService = enableNotificationService;
        }

        public void SetDefaultTab(MainActivity.DefaultTab defaultTab)
        {
            AniDroidSettings.DefaultTab = defaultTab;
        }
    }
}