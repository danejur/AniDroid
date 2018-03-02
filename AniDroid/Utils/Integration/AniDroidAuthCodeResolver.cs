using AniDroid.AniList.Interfaces;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Utils.Integration
{
    internal class AniDroidAuthCodeResolver : IAuthCodeResolver
    {
        private readonly IAniDroidSettings _aniDroidSettings;

        public AniDroidAuthCodeResolver(IAniDroidSettings settings)
        {
            _aniDroidSettings = settings;
        }

        public string AuthCode => _aniDroidSettings.UserAccessCode;
        public bool IsAuthorized => _aniDroidSettings.IsUserAuthenticated;
        public void Invalidate() => _aniDroidSettings.ClearUserAuthentication();
    }
}