﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Google.Android.Material.Snackbar;
using OneOf;

namespace AniDroid.Main
{
    public class MainPresenter : BaseAniDroidPresenter<IMainView>
    {
        public MainPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override async Task Init()
        {
            // TODO: potentially update notifications here, or trigger update at least

            View.SetAuthenticatedNavigationVisibility(AniDroidSettings.IsUserAuthenticated);
            View.OnMainViewSetup();

            if (View.GetVersionCode() > AniDroidSettings.HighestVersionUsed)
            {
                View.DisplayWhatsNewDialog();
                AniDroidSettings.HighestVersionUsed = View.GetVersionCode();
            }

            if ((AniDroidSettings.GenreCache?.Count ?? 0) == 0)
            {
                var genreResult = await AniListService.GetGenreCollectionAsync(default);

                genreResult.Switch(genres => AniDroidSettings.GenreCache = genres)
                    .Switch(error =>
                        View.DisplaySnackbarMessage("Error occurred while caching genres", Snackbar.LengthLong));
            }

            if ((AniDroidSettings.MediaTagCache?.Count ?? 0) == 0)
            {
                var genreResult = await AniListService.GetMediaTagCollectionAsync(default);

                genreResult.Switch(tags => AniDroidSettings.MediaTagCache = tags)
                    .Switch(error =>
                        View.DisplaySnackbarMessage("Error occurred while caching tags", Snackbar.LengthLong));
            }
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> GetNotificationsEnumerable()
        {
            return AniListService.GetAniListNotifications(true, 20);
        }

        public async Task GetUserNotificationCount()
        {
            var countResp = await AniListService.GetAniListNotificationCount(default);

            countResp.Switch(error => {
                    // we're going to force a log out if there was an unauthenticated error on this call
                    if (error.StatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        View.LogoutUser();
                    }

                })
                .Switch(user => View.SetNotificationCount(user.UnreadNotificationCount));
        }

        public bool GetIsUserAuthenticated()
        {
            return AniDroidSettings.IsUserAuthenticated;
        }

        public MainActivity.DefaultTab GetDefaultTab()
        {
            return AniDroidSettings.DefaultTab;
        }
    }
}