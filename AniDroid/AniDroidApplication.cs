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
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace AniDroid
{
#if DEBUG
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@string/AppName", Icon = "@drawable/IconDebug")]
#else
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@string/AppName", Icon = "@drawable/Icon")]
#endif
    public class AniDroidApplication : Application
    {
        protected AniDroidApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Xamarin.Essentials.Platform.Init(this);
            
            Startup.Init();

            AppCenter.Start(Resources.GetString(Resource.String.AppCenterId),
                typeof(Analytics), typeof(Crashes));

            JobManager.Create(this).AddJobCreator(new AniDroidJobCreator(this));

            CreateNotificationsChannel();
        }

        private void CreateNotificationsChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(Resources.GetString(Resource.String.NotificationsChannelId), Resources.GetString(Resource.String.NotificationsChannelName),
                    NotificationImportance.Default);

                channel.EnableVibration(true);
                channel.EnableLights(true);

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}