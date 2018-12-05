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
    [Service(Enabled = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class AniListNotificationService : JobIntentService
    {
        private const int NotificationServiceRequestCode = 1;
        private const int NotificationServiceJobId = 111;
        private const int DefaultServiceIntervalMillis = 1000 * 60 * 30; // 30 min
        private const string NotificationChannelId = "ANILIST_NOTIFICATION_CHANNEL";
        private const string NotificationChannelName = "AniList Notifications";

        private const string NotificationTitle = "{0} new notification{1}";
        private const string NotificationBody = "Tap here to open AniDroid.";

        protected IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule());

        protected override async void OnHandleWork(Intent p0)
        {
            AniListNotificationReceiver.SetAlarm(ApplicationContext);

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
            var notificationBuilder = new NotificationCompat.Builder(ApplicationContext)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(NotificationBody)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(ApplicationContext))
                .SetAutoCancel(true)
                .SetChannelId(NotificationChannelId);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(1, notificationBuilder.Build());
        }

        private static bool ShouldShowNotifications()
        {
            var processInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(processInfo);
            return processInfo.Importance != Importance.Foreground;
        }

        protected static void Enqueue(Context context, Intent intent)
        {
            EnqueueWork(context, Class.FromType(typeof(AniListNotificationService)), NotificationServiceJobId, intent);
        }

        public static void StartNotificationAlarm(Context context)
        {
            EnqueueWork(context, Class.FromType(typeof(AniListNotificationService)), NotificationServiceJobId, new Intent(context, typeof(AniListNotificationService)));
        }

        public static void StopNotificationAlarm(Context context)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(AlarmService);
            alarmManager.Cancel(CreatePendingIntent(context));
        }

        public static void CreateNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(NotificationChannelId, NotificationChannelName,
                    NotificationImportance.Default);

                channel.EnableVibration(true);
                channel.EnableLights(true);

                var notificationManager = (NotificationManager) context.GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private static PendingIntent CreatePendingIntent(Context context) => PendingIntent.GetBroadcast(context, NotificationServiceRequestCode,
            new Intent(context, typeof(AniListNotificationReceiver)), PendingIntentFlags.CancelCurrent);

        [BroadcastReceiver]
        public class AniListNotificationReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                Enqueue(context, intent);
            }

            public static void SetAlarm(Context context)
            {
                var alarmManager = (AlarmManager)context.GetSystemService(AlarmService);
                alarmManager.Set(AlarmType.Rtc, JavaSystem.CurrentTimeMillis() + DefaultServiceIntervalMillis,
                    CreatePendingIntent(context)
                );
            }
        }
    }
}