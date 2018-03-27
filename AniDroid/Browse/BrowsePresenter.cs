using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Browse
{
    public class BrowsePresenter : BaseAniDroidPresenter<IBrowseView>
    {
        public BrowsePresenter(IBrowseView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public void BrowseAniListMedia(BrowseMediaDto browseDto)
        {
            View.ShowMediaSearchResults(AniListService.BrowseMedia(browseDto, 20));
            View.DisplaySnackbarMessage("Browse filtering not yet implemented", Snackbar.LengthShort);
        }

        public override Task Init()
        {
            // TODO: does this need anything?
            return Task.CompletedTask;
        }
    }
}