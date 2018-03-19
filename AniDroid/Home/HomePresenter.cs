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
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Home
{
    public class HomePresenter : BaseAniDroidPresenter<IHomeView>
    {
        public HomePresenter(IHomeView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void GetAniListActivity(bool onlyUserActivity)
        {
            View.ShowUserActivity(AniListService.GetAniListActivity(20), AniDroidSettings.LoggedInUser?.Id ?? 0);
        }
    }
}