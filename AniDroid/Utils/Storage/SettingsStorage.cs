using Android.Content;

namespace AniDroid.Utils.Storage
{
    internal class SettingsStorage : AniDroidStorage
    {
        protected override string Group => "SETTINGS";

        public SettingsStorage(Context c) : base(c)
        {
        }
    }
}