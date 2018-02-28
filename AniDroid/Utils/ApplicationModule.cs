using Android.App;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Service;
using AniDroid.Utils.Integration;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Storage;
using Ninject.Modules;

namespace AniDroid.Utils
{
    public class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAniListServiceConfig>().To<AniDroidAniListServiceConfig>().InSingletonScope();
            Bind<IAuthCodeResolver>().ToMethod(syntax => AniDroidAuthCodeResolver.CreateAuthCodeResolver());
            Bind<IAniListService>().To<AniListService>().InSingletonScope();
            Bind<IAniDroidSettings>().ToConstructor(syntax => new AniDroidSettings(new SettingsStorage(Application.Context))).InSingletonScope();
        }
    }

    public class ApplicationModule<TView, TActivity> : ApplicationModule where TActivity : TView
    {
        private readonly TActivity _activity;

        public ApplicationModule(TActivity activity)
        {
            _activity = activity;
        }

        public override void Load()
        {
            base.Load();
            Bind<TView>().ToConstant(_activity);
        }
    }
}