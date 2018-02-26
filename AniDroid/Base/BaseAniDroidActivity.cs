using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Utils;
using Ninject;
using Square.Picasso;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidActivity<T> : BaseAniDroidActivity, IAniDroidView where T : BaseAniDroidPresenter
    {
        private const string PresenterStateKey = "KEY_PRESENTER_STATE";
        protected IAniDroidView View => this;

        protected T Presenter { get; set; }

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

        public abstract void DisplaySnackbarMessage(string message, int length);
    }

    public abstract class BaseAniDroidActivity : AppCompatActivity
    {
        protected bool HasError { get; set; }
        public sealed override LayoutInflater LayoutInflater => ThemedInflater;

        #region Overrides

        protected sealed override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(GetTheme());

            await OnCreateExtended(savedInstanceState);
        }

        public sealed override void SetContentView(int layoutResId)
        {
            base.SetContentView(layoutResId);
            Cheeseknife.Inject(this);
        }

        #endregion

        #region Theme

        public int GetTheme()
        {
            // TODO: GetTheme needs to return currently configured theme
            return Resource.Style.Dark;
        }

        public int GetThemedResourceId(int attrId)
        {
            var typedVal = new TypedValue();
            Theme.ResolveAttribute(attrId, typedVal, true);
            return typedVal.ResourceId;
        }

        public int GetThemedColor(int attrId)
        {
            var typedVal = new TypedValue();
            Theme.ResolveAttribute(attrId, typedVal, true);
            return new Color(ContextCompat.GetColor(this, typedVal.ResourceId));
        }

        private LayoutInflater ThemedInflater
        {
            get
            {
                using (var contextThemeWrapper = new ContextThemeWrapper(this, GetTheme()))
                {
                    return base.LayoutInflater.CloneInContext(contextThemeWrapper);
                }
            }
        }

        #endregion

        #region Picasso

        private static Picasso PicassoInstance { get; set; }

        public void LoadImage(ImageView imageView, string url, bool showLoading = true)
        {
            var obj = new object();
            lock (obj)
            {
                PicassoInstance = PicassoInstance ?? Picasso.With(ApplicationContext);
            }

            var req = PicassoInstance.Load(url);
            if (showLoading)
            {
                req = req.Placeholder(Android.Resource.Drawable.IcMenuGallery);
            }

            req.Into(imageView);
        }

        #endregion

        #region Abstract

        protected abstract IReadOnlyKernel Kernel { get; }

        public abstract void OnNetworkError();

        public abstract Task OnCreateExtended(Bundle savedInstanceState);

        #endregion

        #region Toolbar

        public sealed override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (HasError)
            {
                // TODO: add error activity recreation
                //if (item.ItemId == Resource.Id.MenuAction_Error_Refresh)
                //{
                //    Recreate();
                //    return true;
                //}
            }

            return MenuItemSelected(item);
        }

        public virtual bool MenuItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        #endregion
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