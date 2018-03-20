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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.Home
{
    public class HomePresenter : BaseAniDroidPresenter<IHomeView>, IAniListActivityPresenter
    {
        public HomePresenter(IHomeView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void GetAniListActivity(bool isFollowingOnly)
        {
            AniDroidSettings.ShowAllAniListActivity = !isFollowingOnly;
            View.ShowUserActivity(AniListService.GetAniListActivity(new AniListActivityDto { IsFollowing = isFollowingOnly }, 20), AniDroidSettings.LoggedInUser?.Id ?? 0);
        }

        public async Task ToggleLike(AniListActivity activity, int activityPosition)
        {
            var toggleResp = await AniListService.ToggleLike(activity.Id, AniList.Models.AniListObject.LikeableType.Activity, default(CancellationToken));

            toggleResp.Switch((IAniListError error) =>
                {
                    View.UpdateActivity(activityPosition, activity);
                    View.DisplaySnackbarMessage("Error occurred while toggling like", Snackbar.LengthLong);
                })
                .Switch(userLikes =>
                {
                    activity.Likes = userLikes;
                    View.UpdateActivity(activityPosition, activity);
                });
        }

        public async Task PostStatusActivity(string text)
        {
            var postResp = await AniListService.PostTextActivity(text, default(CancellationToken));

            postResp.Switch((IAniListError error) => View.DisplaySnackbarMessage("Error occurred while posting status", Snackbar.LengthLong))
                .Switch(activity => View.RefreshActivity());
        }

        public async Task PostActivityReply(AniListActivity activity, int activityPosition, string text)
        {
            var postResp = await AniListService.PostActivityReply(activity.Id, text, default(CancellationToken));

            postResp.Switch((IAniListError error) =>
                {
                    View.UpdateActivity(activityPosition, activity);
                    View.DisplaySnackbarMessage("Error occurred while posting reply", Snackbar.LengthLong);
                })
                .Switch(async reply =>
                {
                    var refreshResp = await AniListService.GetAniListActivityById(activity.Id, default(CancellationToken));

                    refreshResp.Switch((IAniListError error) =>
                        {
                            View.UpdateActivity(activityPosition, activity);
                            View.DisplaySnackbarMessage("Error occurred while refreshing activity", Snackbar.LengthLong);
                        })
                        .Switch(activityResp =>
                        {
                            View.UpdateActivity(activityPosition, activityResp);
                            View.DisplaySnackbarMessage("Reply posted successfully", Snackbar.LengthShort);
                        });
                });
        }
    }
}