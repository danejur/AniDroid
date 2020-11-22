using AniDroid.Base;

namespace AniDroid.Main
{
    public interface IMainView : IAniDroidView
    {
        int GetVersionCode();
        void DisplayWhatsNewDialog();
        void SetAuthenticatedNavigationVisibility(bool isAuthenticated);
        void OnMainViewSetup();
        void SetNotificationCount(int count);
        void LogoutUser();
    }
}