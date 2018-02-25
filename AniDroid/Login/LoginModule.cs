using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Ninject.Modules;

namespace AniDroid.Login
{
    public class LoginModule : NinjectModule
    {
        private readonly ILoginView _view;

        public LoginModule(ILoginView view)
        {
            _view = view;
        }

        public override void Load()
        {
            Bind<ILoginView>().ToConstant(_view);
        }
    }
}