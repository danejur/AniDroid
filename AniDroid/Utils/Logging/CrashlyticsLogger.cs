using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Exception = System.Exception;

namespace AniDroid.Utils.Logging
{
    public class CrashlyticsLogger : IAniDroidLogger
    {
        public void Debug(string tag, string message)
        {
            Crashlytics.Crashlytics.Log((int)LogPriority.Debug, tag, message);
        }

        public void Info(string tag, string message)
        {
            Crashlytics.Crashlytics.Log((int)LogPriority.Info, tag, message);
        }

        public void Warning(string tag, string message)
        {
            Crashlytics.Crashlytics.Log((int)LogPriority.Warn, tag, message);
        }

        public void Error(string tag, string message, Exception exception = null)
        {
            Crashlytics.Crashlytics.Log((int)LogPriority.Error, tag, message);

            if (exception != null)
            {
                Crashlytics.Crashlytics.LogException(Throwable.FromException(exception));
            }
        }
    }
}