using System.Collections.Generic;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidPresenter
    {
        public IAniDroidView View { get; set; }
        public IAniDroidSettings AniDroidSettings { get; }
        public IAniDroidLogger Logger { get; }

        protected IAniListService AniListService { get; }

        protected BaseAniDroidPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger)
        {
            AniDroidSettings = settings;
            Logger = logger;
            AniListService = service;
        }

        //Any initial calls to the view or api calls should go here
        //Do not put initialization in the constructor because Android may need to recreate the presenter from a saved state
        public abstract Task Init();

        public abstract Task BaseInit(IAniDroidView view);

        //These methods are to allow the presenter to be restored properly on Android when the View is killed by the system
        public virtual Task RestoreState(IList<string> savedState)
        {
            return Task.CompletedTask;
        }

        public virtual IList<string> SaveState()
        {
            return new List<string>();
        }
    }

    public abstract class BaseAniDroidPresenter<T> : BaseAniDroidPresenter where T : IAniDroidView
    {
        protected BaseAniDroidPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public new T View { get; set; }

        public sealed override Task BaseInit(IAniDroidView view)
        {
            View = (T)view;

            Init();

            return Task.CompletedTask;
        }
    }
}