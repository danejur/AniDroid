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

namespace AniDroid.Base
{
    public abstract class BaseAniDroidPresenter
    {
        public IAniDroidView View { get; set; }
        protected IAniListService AniListService;

        protected BaseAniDroidPresenter(IAniDroidView view, IAniListService service)
        {
            View = view;
            AniListService = service;
        }

        //Any initial calls to the view or api calls should go here
        //Do not put initialization in the constructor because Android may need to recreate the presenter from a saved state
        public abstract Task Init();

        //These methods are to allow the presenter to be restored properly on Android when the View is killed by the system
        public abstract Task RestoreState(IList<string> savedState);
        public abstract IList<string> SaveState();
    }

    public abstract class BaseAniDroidPresenter<T> : BaseAniDroidPresenter where T : IAniDroidView
    {
        protected BaseAniDroidPresenter(T view, IAniListService service) : base(view, service)
        {
        }

        public new T View => (T) base.View;
    }
}