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
using AniDroid.AniList.Enums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.AniListObject;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.Home
{
    public class HomePresenter : BaseAniDroidPresenter<IHomeView>, IAniListActivityPresenter
    {
        public HomePresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> GetAniListActivity(bool isFollowingOnly)
        {
            AniDroidSettings.ShowAllAniListActivity = !isFollowingOnly;
            return AniListService.GetAniListActivity(new AniListActivityDto {IsFollowing = isFollowingOnly}, 20);
        }

        public int? GetUserId()
        {
            return AniDroidSettings.LoggedInUser?.Id;
        }

        public async Task ToggleActivityLikeAsync(AniListActivity activity, int activityPosition)
        {
            var toggleResp = await AniListService.ToggleLike(activity.Id, LikeableType.Activity, default);

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

        public async Task CreateStatusActivity(string text)
        {
            var postResp = await AniListService.SaveTextActivity(text, null, default);

            postResp.Switch((IAniListError error) => View.DisplaySnackbarMessage("Error occurred while posting status", Snackbar.LengthLong))
                .Switch(activity => View.RefreshActivity());
        }

        public async Task EditStatusActivityAsync(AniListActivity activity, int activityPosition, string updateText)
        {
            var postResp = await AniListService.SaveTextActivity(updateText, activity.Id, default);

            postResp.Switch((IAniListError error) => View.DisplaySnackbarMessage("Error occurred while saving status", Snackbar.LengthLong))
                .Switch(updatedAct => View.UpdateActivity(activityPosition, updatedAct));
        }

        public async Task DeleteActivityAsync(int activityId, int activityPosition)
        {
            var deleteResp = await AniListService.DeleteActivity(activityId, default);

            deleteResp.Switch((IAniListError error) => View.DisplaySnackbarMessage("Error occurred while deleting activity", Snackbar.LengthLong))
                .Switch(deleted => {
                    if (deleted?.Deleted == true)
                    {
                        View.RemoveActivity(activityPosition);
                    }
                    else
                    {
                        View.DisplaySnackbarMessage("Error occurred while deleting activity", Snackbar.LengthLong);
                    }
                });
        }

        public async Task ToggleActivityReplyLikeAsync(ActivityReply activityReply, int activityPosition)
        {
            var toggleResp = await AniListService.ToggleLike(activityReply.Id, LikeableType.ActivityReply, default);

            toggleResp.Switch((IAniListError error) =>
                {
                    View.DisplaySnackbarMessage("Error occurred while toggling like", Snackbar.LengthLong);
                })
                .Switch(userLikes =>
                {
                    activityReply.Likes = userLikes;
                });
        }

        public async Task EditActivityReplyAsync(ActivityReply activityReply, int activityPosition, string updateText)
        {
            var editResp = await AniListService.SaveActivityReply(activityReply.Id, updateText, default);

            editResp.Switch((IAniListError error) =>
                {
                    View.DisplaySnackbarMessage("Error occurred while updating reply", Snackbar.LengthLong);
                })
                .Switch(retReply =>
                {
                    activityReply.Text = retReply.Text;
                    activityReply.Likes = retReply.Likes;
                });
        }

        public async Task<bool> DeleteActivityReplyAsync(ActivityReply activityReply,
            int activityPosition)
        {
            var editResp = await AniListService.DeleteActivityReply(activityReply.Id, default);

            return editResp.Match((IAniListError error) =>
                {
                    View.DisplaySnackbarMessage("Error occurred while deleting reply", Snackbar.LengthLong);

                    return false;
                })
                .Match(deletedResponse => deletedResponse.Deleted);
        }

        public async Task UpdateActivityAsync(AniListActivity activity, int activityPosition)
        {
            var activityResp = await AniListService.GetAniListActivityById(activity.Id, default);

            activityResp.Switch((IAniListError error) => View.DisplaySnackbarMessage("Error occurred while refreshing activity", Snackbar.LengthLong))
                .Switch(updatedAct => View.UpdateActivity(activityPosition, updatedAct));
        }

        public async Task PostActivityReplyAsync(AniListActivity activity, int activityPosition, string text)
        {
            var postResp = await AniListService.PostActivityReply(activity.Id, text, default);

            postResp.Switch((IAniListError error) =>
                {
                    View.UpdateActivity(activityPosition, activity);
                    View.DisplaySnackbarMessage("Error occurred while posting reply", Snackbar.LengthLong);
                })
                .Switch(async reply =>
                {
                    var refreshResp = await AniListService.GetAniListActivityById(activity.Id, default);

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