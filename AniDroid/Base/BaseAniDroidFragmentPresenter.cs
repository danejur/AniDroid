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
    public abstract class BaseAniDroidFragmentPresenter
    {
        public IAniDroidView View { get; set; }
        protected IAniListService AniListService;

        protected BaseAniDroidFragmentPresenter(IAniDroidView view, IAniListService service)
        {
            View = view;
            AniListService = service;
        }

        public abstract Task Init();
    }

    public abstract class BaseAniDroidFragmentPresenter<T> : BaseAniDroidFragmentPresenter where T : IAniDroidView
    {
        protected BaseAniDroidFragmentPresenter(T view, IAniListService service) : base(view, service)
        {
        }

        public new T View => (T)base.View;
    }
}