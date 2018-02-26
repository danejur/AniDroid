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
using AniDroid.AniList.Interfaces;
using Ninject.Activation;

namespace AniDroid.Utils.Integration
{
    internal class AniDroidAuthCodeResolver : IAuthCodeResolver
    {
        public static AniDroidAuthCodeResolver CreateAuthCodeResolver(IContext context)
        {
            return new AniDroidAuthCodeResolver();
        }

        public string AuthCode { get; }
        public bool IsAuthorized => !string.IsNullOrWhiteSpace(AuthCode);
        public void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}