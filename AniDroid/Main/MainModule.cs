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

namespace AniDroid.Main
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMainView>().To<MainActivity>();
        }
    }
}