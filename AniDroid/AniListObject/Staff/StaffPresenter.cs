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
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.AniListObject.Staff
{
    public class StaffPresenter : BaseAniDroidPresenter<IStaffView>
    {
        public StaffPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
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
                    View.SetContentShown(false);
                    View.SetupToolbar($"{staff.Name?.First} {staff.Name?.Last}".Trim());
                    View.SetupStaffView(staff);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetStaffCharactersEnumerable(int staffId, int perPage)
        {
            return AniListService.GetStaffCharacters(staffId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStaffMediaEnumerable(int staffId, MediaType mediaType, int perPage)
        {
            return AniListService.GetStaffMedia(staffId, mediaType, perPage);
        }

        public async Task ToggleFavorite()
        {
            var staffId = View.GetStaffId();
            var favResp = await AniListService.ToggleFavorite(new FavoriteDto { StaffId = staffId },
                default(CancellationToken));

            favResp.Switch(error => View.OnError(error))
                .Switch(favorites => View.SetIsFavorite(favorites.Staff?.Nodes?.Any(x => x.Id == staffId) == true, true));
        }
    }
}