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

namespace AniDroid.Browse
{
    public class BrowsePresenter : BaseAniDroidFragmentPresenter<BrowseFragment>
    {
        public BrowsePresenter(BrowseFragment view, IAniListService service) : base(view, service)
        {
        }

        public override Task Init()
        {
            throw new NotImplementedException();
        }
    }
}