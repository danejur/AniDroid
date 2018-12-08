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
using AniDroid.Jobs;
using Evernote.AndroidJob;

namespace AniDroid
{
#if DEBUG
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@config/AppName", Icon = "@drawable/IconDebug")]
#endif
#if RELEASE
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@config/AppName", Icon = "@drawable/Icon")]
#endif
    public class AniDroidApplication : Application
    {
        protected AniDroidApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            JobManager.Create(this).AddJobCreator(new AniDroidJobCreator(this));

            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();

            CreateNotificationsChannel();
        }

        private void CreateNotificationsChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(Resources.GetString(Resource.Config.NotificationsChannelId), Resources.GetString(Resource.Config.NotificationsChannelName),
                    NotificationImportance.Default);

                channel.EnableVibration(true);
                channel.EnableLights(true);

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}