using AniDroid.Adapters.Base;
using AniDroid.Base;
using AniDroid.Main;

namespace AniDroid.Settings
{
    public interface ISettingsView : IAniDroidView
    {
        void CreateCardTypeSettingItem(BaseRecyclerAdapter.RecyclerCardType cardType);
        void CreateAniDroidThemeSettingItem(BaseAniDroidActivity.AniDroidTheme theme);
        void CreateDisplayBannersSettingItem(bool displayBanners);
        void CreateDisplayUpcomingEpisodeTimeAsCountdownItem(bool displayUpcomingEpisodeTimeAsCountdown);
        void CreateUseSwipeToRefreshHomeScreen(bool useSwipeToRefreshHomeScreen);
        void CreateWhatsNewSettingItem();
        void CreatePrivacyPolicyLinkItem();
        void CreateAboutSettingItem();

        // Auth settings
        void CreateMediaListSettingsItem();
        void CreateEnableNotificationServiceItem(bool enableNotificationService);
        void CreateDefaultTabItem(MainActivity.DefaultTab defaultTab);
    }
}