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
using AniDroid.Base;

namespace AniDroid.Browse
{
    public class BrowseFragment : BaseAniDroidFragment, IAniDroidView
    {
        public void OnNetworkError()
        {
            throw new NotImplementedException();
        }

        public void DisplaySnackbarMessage(string message, int length)
        {
            // TODO: Implement
        }
    }
}