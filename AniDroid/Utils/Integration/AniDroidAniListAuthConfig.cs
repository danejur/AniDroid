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

namespace AniDroid.Utils.Integration
{
    public class AniDroidAniListAuthConfig : IAniListAuthConfig
    {
        private readonly Context _context;

        public AniDroidAniListAuthConfig(Context context)
        {
            _context = context;
        }

        public string ClientId => _context.Resources.GetString(Resource.String.ApiClientId);
        public string ClientSecret => _context.Resources.GetString(Resource.String.ApiClientSecret);
        public string RedirectUri => _context.Resources.GetString(Resource.String.ApiRedirectUri);
        public string AuthTokenUri => _context.Resources.GetString(Resource.String.AniListAuthTokenUri);
    }
}