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

namespace AniDroid.Main
{
    public class MainPresenter : BaseAniDroidPresenter<IMainView>
    {
        public MainPresenter(IMainView view) : base(view)
        {
        }

        public override Task Init()
        {
            // TODO: do something here
            return Task.CompletedTask;
        }

        public override Task RestoreState(IList<string> savedState)
        {
            // TODO: do something here
            return Task.CompletedTask;
        }

        public override IList<string> SaveState()
        {
            // TODO: do something here
            return new List<string>();
        }
    }
}