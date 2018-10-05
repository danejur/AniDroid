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

namespace AniDroid.Utils.Logging
{
    public interface IAniDroidLogger
    {
        void Debug(string tag, string message);
        void Info(string tag, string message);
        void Warning(string tag, string message);
        void Error(string tag, string message, Exception exception = null);
    }
}