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
using AniDroid.Base;
using Ninject;

namespace AniDroid.Main
{
    [Activity(Label = "AniDroid")]
    public class MainActivity : BaseAniDroidActivity<MainPresenter>, IMainView
    {
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule(), new MainModule());

        public override void OnNetworkError()
        {
            throw new NotImplementedException();
        }

        public static void StartActivity(Context context)
        {
            var intent = new Intent(context, typeof(MainActivity));
            context.StartActivity(intent);
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Main);

            await CreatePresenter(savedInstanceState);
        }
    }
}