using Android.Content;

namespace AniDroid.Utils.Storage
{
    internal class AuthSettingsStorage : AniDroidStorage
    {
        protected override string Group => "AUTH_SETTINGS";

        public AuthSettingsStorage(Context c) : base(c)
        {
        }
    }
}