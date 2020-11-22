using AniDroid.AniList.Interfaces;

namespace AniDroid.Utils.Integration
{
    public class AniDroidAniListAuthConfig : IAniListAuthConfig
    {
        public AniDroidAniListAuthConfig(string clientId, string clientSecret, string redirectUri, string authTokenUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
            AuthTokenUri = authTokenUri;
        }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string RedirectUri { get; }
        public string AuthTokenUri { get; }
    }
}