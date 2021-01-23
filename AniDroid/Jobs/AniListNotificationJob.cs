using System;
using System.Collections.Generic;
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
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Evernote.AndroidJob;
using Java.Util.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace AniDroid.Jobs
{
    public class AniListNotificationJob : Job
    {
        public const string Tag = "ANILIST_NOTIFICATION_JOB";

        private const string NotificationTitle = "{0} new notification{1}";
        private const string BasicNotificationContent = "Tap here to open AniDroid.";
        private const string NotificationGroup = "ANILIST_NOTIFICATION_GROUP";
        private const int NotificationId = 1000;

        private readonly Context _context;

        private IAniDroidSettings _aniDroidSettings;
        private IAniListService _aniListService;

        public AniListNotificationJob(Context context)
        {
            _context = context;
            _aniDroidSettings = AniDroidApplication.ServiceProvider.GetService<IAniDroidSettings>();
            _aniListService = AniDroidApplication.ServiceProvider.GetService<IAniListService>();
        }

        protected override Result OnRunJob(Params @params)
        {
            if (AniDroidApplication.ServiceProvider.GetService<IAniDroidSettings>().EnableNotificationService != true)
            {
                DisableJob();
                return Result.Reschedule;
            }

            if (!ShouldShowNotifications())
            {
                return Result.Reschedule;
            }

            var countResp = _aniListService.GetAniListNotificationCount(default).Result;

            countResp.Switch(user =>
            {
                if (user.UnreadNotificationCount > 0)
                {
                    var notificationEnum =
                        _aniListService.GetAniListNotifications(false, Math.Min(user.UnreadNotificationCount, 7));
                    var enumerator = notificationEnum.GetAsyncEnumerator();

                    if (enumerator.MoveNextAsync().Result)
                    {
                        enumerator.Current
                            .Switch(page => CreateDetailedNotification(user.UnreadNotificationCount, page.Data))
                            .Switch(error => CreateBasicNotification(user.UnreadNotificationCount));
                    }
                }
            });

            return Result.Success;
        }

        public static void EnableJob()
        {
            try
            {
                var jobRequests = JobManager.Instance()?.GetAllJobRequestsForTag(Tag);

                if (jobRequests?.Any() == true)
                {
                    return;
                }

                new JobRequest.Builder(Tag)
                    .SetPeriodic(TimeUnit.Minutes.ToMillis(30))
                    .SetUpdateCurrent(true)
                    .SetRequiredNetworkType(JobRequest.NetworkType.Connected)
                    .SetRequirementsEnforced(true)
                    .Build()
                    .Schedule();
            }
            catch
            {
            }
        }

        public static void DisableJob()
        {
            try
            {
                JobManager.Instance().CancelAllForTag(Tag);
            }
            catch
            {
            }
        }

        private void CreateBasicNotification(int notificationCount)
        {
            var notificationBuilder = new NotificationCompat.Builder(_context)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(BasicNotificationContent)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetGroup(NotificationGroup)
                .SetAutoCancel(true)
                .SetDeleteIntent(AniListNotificationJobDismissReciever.CreatePendingIntent(Context))
                .SetChannelId(_context.Resources.GetString(Resource.String.NotificationsChannelId));

            NotificationManagerCompat.From(_context).Notify(NotificationId, notificationBuilder.Build());
        }

        private void CreateDetailedNotification(int notificationCount, ICollection<AniListNotification> notifications)
        {
            var inboxStyle = new NotificationCompat.InboxStyle();

            var notificationTexts =
                notifications.Select(x => BaseAniDroidActivity.FromHtml(x.GetNotificationHtml("fff"))).ToList();

            notificationTexts.ForEach(n => inboxStyle.AddLine(n));

            var notificationBuilder = new NotificationCompat.Builder(_context)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(notificationTexts.First())
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetAutoCancel(true)
                .SetGroup(NotificationGroup)
                .SetChannelId(_context.Resources.GetString(Resource.String.NotificationsChannelId))
                .SetCategory(Notification.CategorySocial)
                .SetDeleteIntent(AniListNotificationJobDismissReciever.CreatePendingIntent(Context))
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
        public class AniListNotificationJobDismissReciever : BroadcastReceiver
        {
            private const int RequestCode = 1001;

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    var service = AniDroidApplication.ServiceProvider.GetService<IAniListService>();
                    var notificationEnum = service.GetAniListNotifications(true, 1);
                    var enumerator = notificationEnum.GetAsyncEnumerator();

                    enumerator.MoveNextAsync().AsTask().Start();
                }
                catch
                {

                }
            }

            public static PendingIntent CreatePendingIntent(Context context)
            {
                return PendingIntent.GetBroadcast(context, RequestCode, new Intent(context, typeof(AniListNotificationJobDismissReciever)),
                    PendingIntentFlags.UpdateCurrent);
            }
        }
    }
}
