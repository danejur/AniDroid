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
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.AniListObject.User
{
    public class UserPresenter : BaseAniDroidPresenter<IUserView>, IAniListActivityPresenter
    {
        public UserPresenter(IUserView view, IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(view, service, settings, logger)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var userId = View.GetUserId();
            var userName = View.GetUserName();

            Logger.Debug("UserPresenter", $"Init started (userId: {userId}, userName: {userName})");

            var userResp = await AniListService.GetUser(userName, userId, default(CancellationToken));

            userResp.Switch(user =>
                {
                    if (AniDroidSettings.IsUserAuthenticated && user.Id != AniDroidSettings.LoggedInUser.Id)
                    {
                        View.SetCanFollow();
                        View.SetCanMessage();
                        View.SetIsFollowing(user.IsFollowing, false);
                    }

                    View.SetShareText(user.Name, user.SiteUrl);
                    View.SetContentShown(false); // TODO: change this to switch based on banner presence
                    View.SetupToolbar(user.Name);
                    View.SetupUserView(user);
                })
                .Switch(error => View.OnError(error));
        }

        public int? GetCurrentUserId()
        {
            return AniDroidSettings.LoggedInUser?.Id;
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> GetUserActivityEnumerable(int userId, int count)
        {
            return AniListService.GetAniListActivity(new AniListActivityDto {UserId = userId}, count);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.User>, IAniListError>> GetUserFollowersEnumerable(int userId, int count)
        {
            return AniListService.GetUserFollowers(userId, AniList.Models.User.UserSort.Username, count);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.User>, IAniListError>> GetUserFollowingEnumerable(int userId, int count)
        {
            return AniListService.GetUserFollowing(userId, AniList.Models.User.UserSort.Username, count);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetUserReviewsEnumerable(int userId,
            int perPage)
        {
            return AniListService.GetUserReviews(userId, perPage);
        }

        public async Task ToggleFollowUser(int userId)
        {
            var toggleResp = await AniListService.ToggleFollowUser(userId, default(CancellationToken));

            toggleResp.Switch((IAniListError error) =>
                    View.DisplaySnackbarMessage("Error occurred while trying to toggle following status",
                        Snackbar.LengthLong))
                .Switch(user => View.SetIsFollowing(user.IsFollowing, true));
        }

        public async Task PostUserMessage(int userId, string message)
        {
            var postResp = await AniListService.PostUserMessage(userId, message, default(CancellationToken));

            postResp.Switch((IAniListError error) =>
                    View.DisplaySnackbarMessage("Error occurred while posting message", Snackbar.LengthLong))
                .Switch(activity =>
                {
                    View.RefreshUserActivity();
                    View.DisplaySnackbarMessage("Message posted successfully", Snackbar.LengthShort);
                });
        }

        public async Task ToggleActivityLikeAsync(AniListActivity activity, int activityPosition)
        {
            var toggleResp = await AniListService.ToggleLike(activity.Id,
                AniList.Models.AniListObject.LikeableType.Activity, default(CancellationToken));

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

        public async Task PostActivityReplyAsync(AniListActivity activity, int activityPosition, string text)
        {
            var postResp = await AniListService.PostActivityReply(activity.Id, text, default(CancellationToken));

            postResp.Switch((IAniListError error) =>
                {
                    View.UpdateActivity(activityPosition, activity);
                    View.DisplaySnackbarMessage("Error occurred while posting reply", Snackbar.LengthLong);
                })
                .Switch(async reply =>
                {
                    var refreshResp =
                        await AniListService.GetAniListActivityById(activity.Id, default(CancellationToken));

                    refreshResp.Switch((IAniListError error) =>
                        {
                            View.UpdateActivity(activityPosition, activity);
                            View.DisplaySnackbarMessage("Error occurred while refreshing activity",
                                Snackbar.LengthLong);
                        })
                        .Switch(activityResp =>
                        {
                            View.UpdateActivity(activityPosition, activityResp);
                            View.DisplaySnackbarMessage("Reply posted successfully", Snackbar.LengthShort);
                        });
                });
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
    }
}