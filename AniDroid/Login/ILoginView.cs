using AniDroid.Base;

namespace AniDroid.Login
{
    public interface ILoginView : IAniDroidView
    {
        string GetAuthCode();
        void OnErrorAuthorizing();
        void OnAuthorized();
    }
}