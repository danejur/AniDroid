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
            Bind<IAuthCodeResolver>().To<AniDroidAuthCodeResolver>().InSingletonScope();
            Bind<IAniListService>().To<AniListService>().InSingletonScope();
            Bind<IAniDroidSettings>().ToConstructor(syntax => new AniDroidSettings(new SettingsStorage(Application.Context))).InSingletonScope();
        }
    }

    public class ApplicationModule<TView, TViewImpl> : ApplicationModule where TViewImpl : TView
    {
        private readonly TViewImpl _viewImpl;

        public ApplicationModule(TViewImpl viewImpl)
        {
            _viewImpl = viewImpl;
        }

        public override void Load()
        {
            base.Load();
            Bind<TView>().ToConstant(_viewImpl);
        }
    }
}