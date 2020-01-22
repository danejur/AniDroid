using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Service;
using AniDroid.AniListObject.Character;
using AniDroid.AniListObject.Media;
using AniDroid.AniListObject.Staff;
using AniDroid.AniListObject.User;
using AniDroid.Browse;
using AniDroid.CurrentSeason;
using AniDroid.Discover;
using AniDroid.Home;
using AniDroid.Login;
using AniDroid.Main;
using AniDroid.MediaList;
using AniDroid.SearchResults;
using AniDroid.Settings;
using AniDroid.TorrentSearch;
using AniDroid.Utils;
using AniDroid.Utils.Integration;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using AniDroid.Utils.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace AniDroid
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void Init()
        {
            var configFile = ExtractResource("AniDroid.appsettings.json", FileSystem.AppDataDirectory);

            var host = new HostBuilder()
                .ConfigureHostConfiguration(c =>
                {
                    // Tell the host configuration where to file the file (this is required for Xamarin apps)
                    c.AddCommandLine(new[] { $"ContentRoot={FileSystem.AppDataDirectory}" });

                    //read in the configuration file!
                    c.AddJsonFile(configFile);
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(l =>
                {
                    l.AddConsole(o =>
                    {
                        //setup a console logger and disable colors since they don't have any colors in VS
                        o.DisableColors = true;
                    });
                })
                .Build();

            //Save our service provider so we can use it later.
            ServiceProvider = host.Services;
        }

        private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddHttpClient();

            services.TryAddSingleton<IAniListAuthConfig>(x => new AniDroidAniListAuthConfig(Application.Context));
            services.TryAddSingleton<IAniDroidLogger, AppCenterLogger>();
            services.TryAddSingleton<IAniDroidSettings>(x => new AniDroidSettings(new SettingsStorage(Application.Context), new AuthSettingsStorage(Application.Context)));
            services.TryAddSingleton<IAuthCodeResolver, AniDroidAuthCodeResolver>();
            services.TryAddSingleton<IAniListServiceConfig>(x => new AniDroidAniListServiceConfig
            {
                BaseUrl = ctx.Configuration["AniListApiUrl"]
            });

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