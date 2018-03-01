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
using AniDroid.Login;
using AniDroid.Main;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Start
{
    [Activity(Label = "AniDroidNew", MainLauncher = true)]
    public class StartActivity : BaseAniDroidActivity
    {
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule());

        public override void OnNetworkError()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public override Task OnCreateExtended(Bundle savedInstanceState)
        {
            // TODO: add checks for data store integrity and other start-up tasks

            MainActivity.StartActivityForResult(this, 0);
            Finish();
            return Task.CompletedTask;
        }
    }
}