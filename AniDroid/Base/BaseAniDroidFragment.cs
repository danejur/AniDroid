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
using Microsoft.Extensions.DependencyInjection;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidFragment<T> : BaseAniDroidFragment where T : BaseAniDroidPresenter
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

    public abstract class BaseAniDroidFragment : Android.Support.V4.App.Fragment, IAniDroidView
    {
        private bool _pendingRecreate;

        protected new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;
        protected new LayoutInflater LayoutInflater => Activity.LayoutInflater;

        public abstract string FragmentName { get; }

        public abstract void OnError(IAniListError error);

        public abstract View CreateView(ViewGroup container, Bundle savedInstanceState);

        public void DisplaySnackbarMessage(string message, int length) => Activity?.DisplaySnackbarMessage(message, length);

        public void DisplayNotYetImplemented() => Activity?.DisplayNotYetImplemented();

        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) => CreateView(container, savedInstanceState);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = HasMenu;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_pendingRecreate)
            {
                _pendingRecreate = false;
                RecreateFragment();
            }
        }

        public virtual bool HasMenu => false;

        public sealed override void OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);
            SetupMenu(menu);
        }

        public virtual void SetupMenu(IMenu menu)
        {
        }

        public void RecreateFragment()
        {
            if (IsDetached)
            {
                return;
            }

            if (IsStateSaved)
            {
                _pendingRecreate = true;
                return;
            }

            FragmentManager.BeginTransaction()
                .Detach(this)
                .Attach(this)
                .Commit();
        }
    }
}
