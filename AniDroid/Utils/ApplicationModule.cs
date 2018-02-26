using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Service;
using AniDroid.Utils.Integration;
using Ninject.Modules;

namespace AniDroid.Utils
{
    public class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAniListServiceConfig>().To<AniDroidAniListServiceConfig>().InSingletonScope();
            Bind<IAuthCodeResolver>().ToMethod(AniDroidAuthCodeResolver.CreateAuthCodeResolver);
            Bind<IAniListService>().To<AniListService>().InSingletonScope();
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