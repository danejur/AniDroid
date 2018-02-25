using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidActivity<T> : BaseAniDroidActivity, IAniDroidView where T : BaseAniDroidPresenter
    {
        private const string PresenterStateKey = "KEY_PRESENTER_STATE";
        protected abstract IReadOnlyKernel Kernel { get; }
        protected IAniDroidView View => this;

        protected T Presenter { get; set; }

        public abstract void OnNetworkError();

        protected async Task CreatePresenter(Bundle savedInstanceState)
        {

            if (GetRetainedPresenter() != null)
            {
                Presenter = GetRetainedPresenter();
                Presenter.View = View;
            }
            else
            {
                Presenter = Kernel.Get<T>();
                if (savedInstanceState != null)
                {
                    await Presenter.RestoreState(savedInstanceState.GetStringArrayList(PresenterStateKey));
                }
                else
                {
                    await Presenter.Init().ConfigureAwait(false);
                }
            }
        }

        protected T GetRetainedPresenter()
        {
            var wrapper = (CustomNonConfigurationWrapper<T>) LastCustomNonConfigurationInstance;
            return wrapper?.Target;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutStringArrayList(PresenterStateKey, Presenter.SaveState());
        }

        public sealed override void SetContentView(int layoutResID)
        {
            base.SetContentView(layoutResID);
            Cheeseknife.Inject(this);
        }
    }

    public abstract class BaseAniDroidActivity : AppCompatActivity
    {
        protected sealed override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(Resource.Style.AniList);

            await OnCreateExtended(savedInstanceState);
        }

        public abstract Task OnCreateExtended(Bundle savedInstanceState);
    }

    public class CustomNonConfigurationWrapper<T> : Java.Lang.Object
    {
        public readonly T Target;

        public CustomNonConfigurationWrapper(T target)
        {
            Target = target;
        }
    }
}