using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.AniListObject.Staff
{
    public class StaffPresenter : BaseAniDroidPresenter<IStaffView>
    {
        public StaffPresenter(IStaffView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var staffId = View.GetStaffId();
            var staffResp = await AniListService.GetStaffById(staffId, default(CancellationToken));

            staffResp.Switch(staff =>
                {
                    View.SetIsFavorite(staff.IsFavourite);
                    View.SetShareText(staff.Name?.GetFormattedName(), staff.SiteUrl);
                    View.SetContentShown();
                    View.SetupToolbar($"{staff.Name?.First} {staff.Name?.Last}".Trim());
                    View.SetupStaffView(staff);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Character.Edge>> GetStaffCharactersEnumerable(int staffId, int perPage)
        {
            return AniListService.GetStaffCharacters(staffId, perPage);
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Media.Edge>> GetStaffMediaEnumerable(int staffId, AniList.Models.Media.MediaType mediaType, int perPage)
        {
            return AniListService.GetStaffMedia(staffId, mediaType, perPage);
        }
    }
}