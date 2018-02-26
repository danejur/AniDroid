using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Main
{
    [Activity(Label = "AniDroid")]
    public class MainActivity : BaseAniDroidActivity<MainPresenter>, IMainView
    {
        [InjectView(Resource.Id.Main_NavigationView)]
        private NavigationView _navigationView;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IMainView, MainActivity>(this));

        public override void OnNetworkError()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public void SetAuthenticatedNavigationVisibility(bool isAuthenticated)
        {
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Anime)?.SetVisible(isAuthenticated);
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Manga)?.SetVisible(isAuthenticated);
            _navigationView?.Menu?.FindItem(Resource.Id.Menu_Navigation_Home)?.SetVisible(isAuthenticated);
        }

        public static void StartActivity(Context context)
        {
            var intent = new Intent(context, typeof(MainActivity));
            context.StartActivity(intent);
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            // TODO: Implement
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Main);

            await CreatePresenter(savedInstanceState);

            SearchResultsActivity.StartAniListSearchResultsActivity(this, SearchResultsActivity.AniListSearchTypes.Anime, "test");
        }
    }
}