using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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