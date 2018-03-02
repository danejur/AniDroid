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
using AniDroid.Adapters.Base;
using AniDroid.Base;

namespace AniDroid.Utils.Interfaces
{
    public interface IAniDroidSettings
    {
        BaseRecyclerAdapter.CardType CardType { get; set; }
        BaseAniDroidActivity.AniDroidTheme Theme { get; set; }
        string UserAccessCode { get; set; }
        bool IsUserAuthenticated { get; }
        void ClearUserAuthentication();
    }
}