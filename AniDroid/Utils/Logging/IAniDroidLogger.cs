using System;

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