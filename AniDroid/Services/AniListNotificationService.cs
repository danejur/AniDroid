using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Main;
using AniDroid.Utils;
using Java.Lang;
using Ninject;

namespace AniDroid.Services
{
    [Service(Enabled = true)]
    public class AniListNotificationService : IntentService
    {
        private const int NotificationServiceRequestCode = 1;
        private const int DefaultServiceIntervalMillis = 1000 * 60 * 30; // 30 min

        private const string NotificationTitle = "{0} new notification{1}";
        private const string NotificationBody = "Tap here to open AniDroid.";

        protected IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule());

        protected override async void OnHandleIntent(Intent intent)
        {
            if (!ShouldShowNotifications())
            {
                return;
            }

            var aniListService = Kernel.Get<IAniListService>();

            var countResp = await aniListService.GetAniListNotificationCount(default);

            countResp.Switch(user =>
            {
                if (user.UnreadNotificationCount > 0)
                {
                    CreateNotification(user.UnreadNotificationCount);
                }
            });
        }

        private void CreateNotification(int notificationCount)
        {
            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(NotificationBody)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(ApplicationContext))
                .SetAutoCancel(true);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(1, notificationBuilder.Build());
        }

        public static PendingIntent CreateNotificationCheckPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(AniListNotificationService));
            return PendingIntent.GetService(context, NotificationServiceRequestCode, intent, PendingIntentFlags.UpdateCurrent);
        }

        private static bool ShouldShowNotifications()
        {
            var processInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(processInfo);
            return processInfo.Importance != Importance.Foreground;
        }

        public class Alarm
        {
            public static void StartNotificationAlarm(Context context)
            {
                var alarmManager = (AlarmManager)context.GetSystemService(AlarmService);
                alarmManager.SetRepeating(AlarmType.Rtc, JavaSystem.CurrentTimeMillis(), DefaultServiceIntervalMillis,
                    CreateNotificationCheckPendingIntent(context));
            }

            public static void StopNotificationAlarm(Context context)
            {
                var alarmManager = (AlarmManager)context.GetSystemService(AlarmService);
                alarmManager.Cancel(CreateNotificationCheckPendingIntent(context));
            }
        }
    }
}