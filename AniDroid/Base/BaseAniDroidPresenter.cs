using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidPresenter
    {
        public IAniDroidView View { get; set; }
        protected IAniListService AniListService { get; }
        protected IAniDroidSettings AniDroidSettings { get; }
        protected IAniDroidLogger Logger { get; }

        protected BaseAniDroidPresenter(IAniDroidView view, IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger)
        {
            View = view;
            AniListService = service;
            AniDroidSettings = settings;
            Logger = logger;
        }

        //Any initial calls to the view or api calls should go here
        //Do not put initialization in the constructor because Android may need to recreate the presenter from a saved state
        public abstract Task Init();

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
        protected BaseAniDroidPresenter(T view, IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(view, service, settings, logger)
        {
        }

        public new T View => (T) base.View;
    }
}