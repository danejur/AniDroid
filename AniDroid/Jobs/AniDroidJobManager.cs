using System;
using Android.Content;
using AndroidX.Work;

namespace AniDroid.Jobs
{
    public static class AniDroidJobManager
    {
        public static void EnableAniListNotificationJob(Context context)
        {
            var workReq = new PeriodicWorkRequest.Builder(typeof(AniListNotificationJobWorker), TimeSpan.FromMinutes(30)).AddTag(AniListNotificationJobWorker.Tag).Build();
            WorkManager.GetInstance(context).EnqueueUniquePeriodicWork(AniListNotificationJobWorker.Tag, ExistingPeriodicWorkPolicy.Keep, workReq);
        }

        public static void DisableAniListNotificationJob(Context context)
        {
            WorkManager.GetInstance(context).CancelAllWorkByTag(AniListNotificationJobWorker.Tag);
        }
    }
}