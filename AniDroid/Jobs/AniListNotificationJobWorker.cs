using System;
using Android.App;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Core.App;
using AndroidX.Work;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AniDroid.Jobs
{
    public class AniListNotificationJobWorker : Worker
    {
        public const string Tag = "ANILIST_NOTIFICATION_JOB";

        private const string NotificationTitle = "{0} new notification{1}";
        private const string BasicNotificationContent = "Tap here to open AniDroid.";
        private const string NotificationGroup = "ANILIST_NOTIFICATION_GROUP";
        private const string ChannelId = "AniList Notifications";
        private const int NotificationId = 1000;

        private readonly Context _context;

        public AniListNotificationJobWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
            _context = context;
        }

        public override Result DoWork()
        {
            if (AniDroidApplication.ServiceProvider.GetService<IAniDroidSettings>()?.EnableNotificationService != true)
            {
                AniDroidJobManager.DisableAniListNotificationJob(_context);
                return Result.InvokeFailure();
            }

            if (!ShouldShowNotifications())
            {
                // return success instead of retry to so we don't trigger a notification soon after the user leaves the app
                return Result.InvokeSuccess();
            }

            var aniListService = AniDroidApplication.ServiceProvider.GetService<IAniListService>();

            if (aniListService == null)
            {
                return Result.InvokeFailure();
            }

            var countResp = aniListService.GetAniListNotificationCount(default).Result;

            countResp.Switch(user =>
            {
                if (user.UnreadNotificationCount > 0)
                {
                    var notificationEnum =
                        aniListService.GetAniListNotifications(false, Math.Min(user.UnreadNotificationCount, 7));
                    var enumerator = notificationEnum.GetAsyncEnumerator();

                    if (enumerator.MoveNextAsync().Result)
                    {
                        enumerator.Current
                            .Switch(page => CreateDetailedNotification(user.UnreadNotificationCount, page.Data))
                            .Switch(error => CreateBasicNotification(user.UnreadNotificationCount));
                    }
                }
            });

            return Result.InvokeSuccess();
        }

        private void CreateBasicNotification(int notificationCount)
        {
            var notificationBuilder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(BasicNotificationContent)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetGroup(NotificationGroup)
                .SetAutoCancel(true)
                .SetDeleteIntent(AniListNotificationJobDismissReceiver.CreatePendingIntent(_context))
                .SetChannelId(_context.Resources?.GetString(Resource.String.NotificationsChannelId));

            NotificationManagerCompat.From(_context).Notify(NotificationId, notificationBuilder.Build());
        }

        private void CreateDetailedNotification(int notificationCount, IEnumerable<AniListNotification> notifications)
        {
            var inboxStyle = new NotificationCompat.InboxStyle();

            var notificationTexts =
                notifications.Select(x => BaseAniDroidActivity.FromHtml(x.GetNotificationHtml("fff"))).ToList();

            notificationTexts.ForEach(n => inboxStyle.AddLine(n));

            var notificationBuilder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(notificationTexts.First())
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetAutoCancel(true)
                .SetGroup(NotificationGroup)
                .SetChannelId(_context.Resources?.GetString(Resource.String.NotificationsChannelId))
                .SetCategory(Notification.CategorySocial)
                .SetDeleteIntent(AniListNotificationJobDismissReceiver.CreatePendingIntent(_context))
                .SetStyle(inboxStyle);

            NotificationManagerCompat.From(_context).Notify(NotificationId, notificationBuilder.Build());
        }

        private static bool ShouldShowNotifications()
        {
            var processInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(processInfo);
            return processInfo.Importance != Importance.Foreground;
        }

        [BroadcastReceiver]
        public class AniListNotificationJobDismissReceiver : BroadcastReceiver
        {
            private const int RequestCode = 1001;

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    var service = AniDroidApplication.ServiceProvider.GetService<IAniListService>();
                    var notificationEnum = service?.GetAniListNotifications(true, 1);
                    var enumerator = notificationEnum?.GetAsyncEnumerator();

                    enumerator?.MoveNextAsync().AsTask().Start();
                }
                catch (Exception e)
                {
                    var activityContext = context as BaseAniDroidActivity;

                    activityContext?.Logger?.Error(nameof(AniListNotificationJobDismissReceiver),
                        "Error occurred while removing AniList notifications", e);
                }
            }

            public static PendingIntent CreatePendingIntent(Context context)
            {
                return PendingIntent.GetBroadcast(context, RequestCode, new Intent(context, typeof(AniListNotificationJobDismissReceiver)),
                    PendingIntentFlags.UpdateCurrent);
            }
        }
    }
}