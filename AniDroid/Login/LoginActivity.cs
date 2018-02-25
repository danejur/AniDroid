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
        [InjectView(Resource.Id.Login_LoginButton)]
        private Button _loginButton;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule(), new LoginModule(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Login);
            await CreatePresenter(savedInstanceState);
        }

        public override void OnNetworkError()
        {
            throw new NotImplementedException();
        }

        public void LoginButtonClick()
        {
            _loginButton.Text = "Clicked!";
        }

        public static void StartActivity(Context context)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            context.StartActivity(intent);
        }
    }
}