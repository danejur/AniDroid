using AniDroid.AniList.Interfaces;

namespace AniDroid.Utils.Integration
{
    internal class AniDroidAniListServiceConfig : IAniListServiceConfig
    {
        public AniDroidAniListServiceConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
    }
}