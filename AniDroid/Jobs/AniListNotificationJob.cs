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
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Evernote.AndroidJob;
using Java.Util.Concurrent;
using Ninject;

namespace AniDroid.Jobs
{
    public class AniListNotificationJob : Job
    {
        public const string Tag = "ANILIST_NOTIFICATION_JOB";

        private const string NotificationTitle = "{0} new notification{1}";
        private const string BasicNotificationContent = "Tap here to open AniDroid.";

        protected IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule());

        private readonly Context _context;

        public AniListNotificationJob(Context context)
        {
            _context = context;
        }

        protected override Result OnRunJob(Params @params)
        {
            if (Kernel.Get<IAniDroidSettings>().EnableNotificationService != true)
            {
                DisableJob();
                return Result.Reschedule;
            }

            if (!ShouldShowNotifications())
            {
                return Result.Reschedule;
            }

            var aniListService = Kernel.Get<IAniListService>();

            var countResp = aniListService.GetAniListNotificationCount(default).Result;

            countResp.Switch(user =>
            {
                if (user.UnreadNotificationCount > 0)
                {
                    var notificationEnum =
                        aniListService.GetAniListNotifications(false, Math.Min(user.UnreadNotificationCount, 7));
                    var enumerator = notificationEnum.GetEnumerator();

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

        public static void DisableJob()
        {
            JobManager.Instance().CancelAllForTag(Tag);
        }

        private void CreateBasicNotification(int notificationCount)
        {
            var notificationBuilder = new NotificationCompat.Builder(_context)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(BasicNotificationContent)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetAutoCancel(true)
                .SetChannelId(_context.Resources.GetString(Resource.Config.NotificationsChannelId));

            var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, notificationBuilder.Build());
        }

        private void CreateDetailedNotification(int notificationCount, ICollection<AniListNotification> notifications)
        {
            var inboxStyle = new NotificationCompat.InboxStyle();

            foreach (var notification in notifications)
            {
                inboxStyle.AddLine(BaseAniDroidActivity.FromHtml(notification.GetNotificationHtml("fff")));
            }

            var notificationBuilder = new NotificationCompat.Builder(_context)
                .SetContentTitle(string.Format(NotificationTitle, notificationCount, notificationCount > 1 ? "s" : ""))
                .SetContentText(BasicNotificationContent)
                .SetSmallIcon(Resource.Drawable.IconTransparent)
                .SetContentIntent(MainActivity.CreatePendingIntentToOpenNotifications(_context))
                .SetAutoCancel(true)
                .SetChannelId(_context.Resources.GetString(Resource.Config.NotificationsChannelId))
                .SetStyle(inboxStyle);

            var notificationManager = (NotificationManager) _context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, notificationBuilder.Build());
        }

        private static bool ShouldShowNotifications()
        {
            var processInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(processInfo);
            return processInfo.Importance != Importance.Foreground;
        }
    }
}