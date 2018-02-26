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
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Login
{
    [Activity(Label = "Login")]
    public class LoginActivity : BaseAniDroidActivity<LoginPresenter>, ILoginView
    {
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ILoginView, LoginActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Login);
            await CreatePresenter(savedInstanceState);
        }

        public override void OnNetworkError()
        {
            throw new NotImplementedException();
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            // TODO: Implement
        }

        public static void StartActivity(Context context)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            context.StartActivity(intent);
        }
    }
}