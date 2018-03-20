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
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using OneOf;

namespace AniDroid.AniListObject.User
{
    public class UserPresenter : BaseAniDroidPresenter<IUserView>, IAniListActivityPresenter
    {
        public UserPresenter(IUserView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var userId = View.GetUserId();
            var userName = View.GetUserName();

            var userResp = await AniListService.GetUser(userName, userId, default(CancellationToken));

            userResp.Switch(user =>
                {
                    View.SetShareText(user.Name, user.SiteUrl);
                    View.SetContentShown(false); // TODO: change this to switch based on banner presence
                    View.SetupToolbar(user.Name);
                    View.SetupUserView(user);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> GetUserActivityEnumrable(int userId, int count)
        {
            return AniListService.GetAniListActivity(new AniListActivityDto {UserId = userId}, count);
        }

        public async Task ToggleLike(AniListActivity activity, int activityPosition)
        {
            //var toggleResp = await AniListService.ToggleLike(activity.Id, AniList.Models.AniListObject.LikeableType.Activity, default(CancellationToken));

            //toggleResp.Switch((IAniListError error) =>
            //{
            //    View.UpdateActivity(activityPosition, activity);
            //    View.DisplaySnackbarMessage("Error occurred while toggling like", Snackbar.LengthLong);
            //})
            //    .Switch(userLikes =>
            //    {
            //        activity.Likes = userLikes;
            //        View.UpdateActivity(activityPosition, activity);
            //    });
        }

        public async Task PostActivityReply(AniListActivity activity, int activityPosition, string text)
        {
            //var postResp = await AniListService.PostActivityReply(activity.Id, text, default(CancellationToken));

            //postResp.Switch((IAniListError error) =>
            //{
            //    View.UpdateActivity(activityPosition, activity);
            //    View.DisplaySnackbarMessage("Error occurred while posting reply", Snackbar.LengthLong);
            //})
            //    .Switch(async reply =>
            //    {
            //        var refreshResp = await AniListService.GetAniListActivityById(activity.Id, default(CancellationToken));

            //        refreshResp.Switch((IAniListError error) =>
            //        {
            //            View.UpdateActivity(activityPosition, activity);
            //            View.DisplaySnackbarMessage("Error occurred while refreshing activity", Snackbar.LengthLong);
            //        })
            //            .Switch(activityResp =>
            //            {
            //                View.UpdateActivity(activityPosition, activityResp);
            //                View.DisplaySnackbarMessage("Reply posted successfully", Snackbar.LengthShort);
            //            });
            //    });
        }
    }
}