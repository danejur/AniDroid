using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Interfaces;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Square.Picasso;
using Microsoft.Extensions.DependencyInjection;

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
                Presenter = AniDroidApplication.ServiceProvider.GetService<T>();
                await Presenter.BaseInit(View).ConfigureAwait(false);
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
            outState.PutStringArrayList(PresenterStateKey, Presenter?.SaveState());
        }

        public abstract override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort);

        public sealed override void DisplayNotYetImplemented()
        {
            DisplaySnackbarMessage("Not Yet Implemented", Snackbar.LengthShort);
        }

        public sealed override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (HasError)
            {
                MenuInflater.Inflate(Resource.Menu.Error_ActionBar, menu);
                return true;
            }

            return SetupMenu(menu);
        }

        public sealed override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (HasError)
            {
                if (item.ItemId == Resource.Id.Menu_Error_Refresh)
                {
                    Presenter.BaseInit(View).ConfigureAwait(false).GetAwaiter();
                    return true;
                }
            }

            return MenuItemSelected(item);
        }

        public sealed override void Recreate()
        {
            Presenter = null;
            base.Recreate();
        }
    }

    [Activity(Label = "@string/AppName")]
    public abstract class BaseAniDroidActivity : AppCompatActivity
    {
        public const int ObjectBrowseRequestCode = 9;

        private static AniDroidTheme _theme;
        public IAniDroidSettings Settings { get; private set; }
        public IAniDroidLogger Logger { get; private set; }
        protected bool HasError { get; set; }
        public sealed override LayoutInflater LayoutInflater => ThemedInflater;
        public BaseRecyclerAdapter.RecyclerCardType CardType { get; private set; }

        #region Overrides

        protected sealed override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Logger = AniDroidApplication.ServiceProvider.GetService<IAniDroidLogger>();
            Settings = AniDroidApplication.ServiceProvider.GetService<IAniDroidSettings>();
            _theme = Settings.Theme;
            CardType = Settings.CardType;
            SetTheme(GetThemeResource());

            await OnCreateExtended(savedInstanceState);
        }

        public sealed override void SetContentView(int layoutResId)
        {
            base.SetContentView(layoutResId);
            Cheeseknife.Inject(this);
        }

        #endregion

        #region Theme

        public override Resources.Theme Theme {
            get
            {
                var theme = GetThemeResource();

                var baseTheme = base.Theme;
                baseTheme.ApplyStyle(theme, true);

                return baseTheme;
            }
        }

        public int GetThemeResource()
        {
            var theme = Resource.Style.AniList;

            switch (_theme)
            {
                case AniDroidTheme.Black:
                    theme = Resource.Style.Black;
                    break;
                case AniDroidTheme.AniListDark:
                    theme = Resource.Style.AniListDark;
                    break;
                case AniDroidTheme.Dark:
                    theme = Resource.Style.Dark;
                    break;
            }

            return theme;
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
                using (var contextThemeWrapper = new ContextThemeWrapper(this, GetThemeResource()))
                {
                    return base.LayoutInflater.CloneInContext(contextThemeWrapper);
                }
            }
        }

        public enum AniDroidTheme
        {
            AniList = 0,
            Black = 1,
            AniListDark = 2,
            Dark = 3
        }

        #endregion

        #region Context Utils

        public void LoadImage(ImageView imageView, string url, bool showLoading = true)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var req = Picasso.With(this).Load(url);

            if (showLoading)
            {
                req = req.Placeholder(Resource.Drawable.svg_image);
            }

            req.Into(imageView);
        }

        public static ISpanned FromHtml(string source)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return Build.VERSION.SdkInt >= BuildVersionCodes.N ? Html.FromHtml(source ?? "", FromHtmlOptions.ModeLegacy) : Html.FromHtml(source ?? "");
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public float GetDimensionFromDp(float dpVal)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dpVal, Resources.DisplayMetrics);
        }

        #endregion

        #region Abstract

        public abstract void OnError(IAniListError error);

        public abstract Task OnCreateExtended(Bundle savedInstanceState);

        public abstract void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort);

        public abstract void DisplayNotYetImplemented();

        #endregion

        #region Toolbar

        public virtual bool MenuItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public virtual bool SetupMenu(IMenu menu)
        {
            return true;
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