using System;
using System.IO;
using System.Reflection;
using Android.App;
using Android.OS;
using Android.Runtime;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Service;
using AniDroid.AniListObject.Character;
using AniDroid.AniListObject.Media;
using AniDroid.AniListObject.Staff;
using AniDroid.AniListObject.Studio;
using AniDroid.AniListObject.User;
using AniDroid.Browse;
using AniDroid.CurrentSeason;
using AniDroid.Discover;
using AniDroid.Favorites;
using AniDroid.Home;
using AniDroid.Jobs;
using AniDroid.Login;
using AniDroid.Main;
using AniDroid.MediaList;
using AniDroid.SearchResults;
using AniDroid.Settings;
using AniDroid.Settings.MediaListSettings;
using AniDroid.TorrentSearch;
using AniDroid.Utils;
using AniDroid.Utils.Integration;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using AniDroid.Utils.Storage;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace AniDroid
{
#if DEBUG
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@string/AppName", Icon = "@drawable/IconDebug")]
#else
    [Application(AllowBackup = true, Theme = "@style/AniList", Label= "@string/AppName", Icon = "@drawable/Icon")]
#endif
    public class AniDroidApplication : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }

        protected AniDroidApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Platform.Init(this);

            var serviceProvider = InitServiceProvider();

            var appCenterId = serviceProvider.GetService<IConfiguration>()["AppCenterId"];

            AppCenter.Start(appCenterId,
                typeof(Analytics), typeof(Crashes));

            //JobManager.Create(this).AddJobCreator(new AniDroidJobCreator(this));

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

        public static IServiceProvider InitServiceProvider()
        {
            var configFile = ExtractResource("AniDroid.appsettings.json", FileSystem.AppDataDirectory);
            var secretConfigFile = ExtractResource("AniDroid.appsettings.secret.json", FileSystem.AppDataDirectory);

            var host = new HostBuilder()
                .UseContentRoot(FileSystem.AppDataDirectory)
                .ConfigureHostConfiguration(c =>
                {

                    // Tell the host configuration where to file the file (this is required for Xamarin apps)
                    //c.AddCommandLine(new[] { $"ContentRoot={FileSystem.AppDataDirectory}" });

                    c.AddJsonFile(configFile);

                    c.AddJsonFile(secretConfigFile);
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(l =>
                {
                    //l.AddConsole(o =>
                    //{
                    //    //setup a console logger and disable colors since they don't have any colors in VS
                    //    o.DisableColors = true;
                    //});
                })
                .Build();

            //Save our service provider so we can use it later.
            return ServiceProvider = host.Services;
        }

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddHttpClient();

            services.TryAddSingleton<IAniDroidLogger, AppCenterLogger>();

            services.TryAddSingleton<IAniListAuthConfig>(x => new AniDroidAniListAuthConfig(ctx.Configuration["ApiConfiguration:ClientId"],
                ctx.Configuration["ApiConfiguration:ClientSecret"], ctx.Configuration["ApiConfiguration:RedirectUrl"],
                ctx.Configuration["ApiConfiguration:AuthUrl"]));
            services.TryAddSingleton<IAniDroidSettings>(x => new AniDroidSettings(new SettingsStorage(Application.Context), new AuthSettingsStorage(Application.Context)));
            services.TryAddSingleton<IAuthCodeResolver, AniDroidAuthCodeResolver>();
            services.TryAddSingleton<IAniListServiceConfig>(x => new AniDroidAniListServiceConfig(ctx.Configuration["ApiConfiguration:BaseUrl"]));

            services.TryAddTransient<IAniListService, AniListService>();

            ConfigurePresenters(services);
        }

        private static void ConfigurePresenters(IServiceCollection services)
        {
            services.TryAddTransient<MainPresenter>();
            services.TryAddTransient<MediaListPresenter>();
            services.TryAddTransient<MediaPresenter>();
            services.TryAddTransient<CharacterPresenter>();
            services.TryAddTransient<StaffPresenter>();
            services.TryAddTransient<BrowsePresenter>();
            services.TryAddTransient<CurrentSeasonPresenter>();
            services.TryAddTransient<DiscoverPresenter>();
            services.TryAddTransient<HomePresenter>();
            services.TryAddTransient<LoginPresenter>();
            services.TryAddTransient<SearchResultsPresenter>();
            services.TryAddTransient<SettingsPresenter>();
            services.TryAddTransient<TorrentSearchPresenter>();
            services.TryAddTransient<UserPresenter>();
            services.TryAddTransient<MediaListSettingsPresenter>();
            services.TryAddTransient<StudioPresenter>();
            services.TryAddTransient<FavoritesPresenter>();

        }

        private static string ExtractResource(string filename, string location)
        {
            var a = Assembly.GetExecutingAssembly();
            using var resFilestream = a.GetManifestResourceStream(filename);

            if (resFilestream != null)
            {
                using var stream = File.Create(Path.Combine(location, filename));
                resFilestream.CopyTo(stream);
            }

            return Path.Combine(location, filename);
        }
    }
}