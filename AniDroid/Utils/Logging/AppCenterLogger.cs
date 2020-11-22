using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Exception = System.Exception;

namespace AniDroid.Utils.Logging
{
    public class AppCenterLogger : IAniDroidLogger
    {
        public void Debug(string tag, string message)
        {
            Analytics.TrackEvent("Debug", new Dictionary<string, string>
            {
                {
                    "Message", message
                }
            });
        }

        public void Info(string tag, string message)
        {
            Analytics.TrackEvent("Info", new Dictionary<string, string>
            {
                {
                    "Message", message
                }
            });
        }

        public void Warning(string tag, string message)
        {
            Analytics.TrackEvent("Warning", new Dictionary<string, string>
            {
                {
                    "Message", message
                }
            });
        }

        public void Error(string tag, string message, Exception exception = null)
        {
            Analytics.TrackEvent("Error", new Dictionary<string, string>
            {
                {
                    "Message", message
                },
                {
                    "Exception", exception?.Message
                },
                {
                    "StackTrace", exception?.StackTrace
                }
            });
        }
    }
}