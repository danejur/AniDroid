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

namespace AniDroid.Utils.Listeners
{
    public class InterceptClickListener : Java.Lang.Object, View.IOnClickListener
    {
        public Action ActionToInvoke { get; }

        public InterceptClickListener(Action action)
        {
            ActionToInvoke = action;
        }

        public void OnClick(View v)
        {
            ActionToInvoke.Invoke();
        }
    }
}