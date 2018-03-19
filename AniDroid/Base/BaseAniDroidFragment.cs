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
using Ninject;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidFragment<T> : BaseAniDroidFragment where T : BaseAniDroidPresenter
    {
        protected T Presenter { get; set; }

        protected async Task CreatePresenter(Bundle savedInstanceState)
        {
            Presenter = Kernel.Get<T>();
            await Presenter.Init().ConfigureAwait(false);
        }
    }

    public abstract class BaseAniDroidFragment : Android.Support.V4.App.Fragment, IAniDroidView
    {
        protected new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;
        protected new LayoutInflater LayoutInflater => Activity.LayoutInflater;

        public abstract string FragmentName { get; }

        protected abstract IReadOnlyKernel Kernel { get; }

        public abstract void OnError(IAniListError error);

        public abstract View CreateView(ViewGroup container, Bundle savedInstanceState);

        public void DisplaySnackbarMessage(string message, int length) => Activity.DisplaySnackbarMessage(message, length);

        public void DisplayNotYetImplemented() => Activity.DisplayNotYetImplemented();

        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) => CreateView(container, savedInstanceState);
    }
}
