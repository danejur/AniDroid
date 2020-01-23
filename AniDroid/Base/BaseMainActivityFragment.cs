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
using AniDroid.Main;
using Microsoft.Extensions.DependencyInjection;

namespace AniDroid.Base
{
    public abstract class BaseMainActivityFragment<T> : BaseMainActivityFragment where T : BaseAniDroidPresenter
    {
        protected T Presenter { get; set; }

        protected async Task CreatePresenter(Bundle savedInstanceState)
        {
            if (Presenter != null)
            {
                return;
            }

            Presenter = Startup.ServiceProvider.GetService<T>();
            await Presenter.BaseInit(this).ConfigureAwait(false);
        }
    }

    public abstract class BaseMainActivityFragment : BaseAniDroidFragment
    {
        protected new MainActivity Activity => base.Activity as MainActivity;

        protected abstract void SetInstance(BaseMainActivityFragment instance);

        public abstract void ClearState();

        public abstract View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetInstance(this);
        }

        public sealed override View CreateView(ViewGroup container, Bundle savedInstanceState)
        {
            Activity.ShowSearchButton();
            return CreateMainActivityFragmentView(container, savedInstanceState);
        }

        public virtual Action GetSearchFabAction() => null;
    }
}