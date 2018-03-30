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
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.MediaList
{
    public class MediaListPresenter : BaseAniDroidPresenter<IMediaListView>, IAniListMediaListEditPresenter
    {
        public MediaListPresenter(IMediaListView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public async Task GetMediaLists()
        {
            var mediaListResp = await AniListService.GetUserMediaList(AniDroidSettings.LoggedInUser?.Id ?? 0,
                View.GetMediaType(), AniDroidSettings.GroupCompletedLists, default(CancellationToken));

            mediaListResp.Switch(error => View.OnError(error))
                .Switch(mediaLists => View.SetCollection(mediaLists));
        }

        public async Task SaveMediaListEntry(MediaListEditDto editDto)
        {
            //View.SetMediaListSaving();

            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
                {
                    View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                })
                .Switch(error =>
                {
                    View.DisplaySnackbarMessage("Error occurred while saving list entry", Snackbar.LengthLong);
                    //View.ShowMediaListEditDialog(new AniList.Models.Media.MediaList
                    //{
                    //    Score = editDto.Score ?? 0,
                    //    Progress = editDto.Progress,
                    //    ProgressVolumes = editDto.ProgressVolumes,
                    //    Status = editDto.Status,
                    //    Notes = editDto.Notes,
                    //    Repeat = editDto.Repeat
                    //});
                });
        }

        public async Task IncreaseMediaProgress(Media.MediaList mediaListToUpdate)
        {
            var editDto = new MediaListEditDto
            {
                MediaId = mediaListToUpdate.Media.Id,
                Progress = (mediaListToUpdate.Progress ?? 0) + 1
            };

            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
                {
                    View.DisplaySnackbarMessage($"Updated progress for {mediaList.Media.Title.UserPreferred}", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                })
                .Switch(error =>
                {
                    View.DisplaySnackbarMessage("Error occurred while saving list entry", Snackbar.LengthLong);
                    View.ResetMediaListItem(mediaListToUpdate.Media.Id);
                });
        }

        public async Task CompleteMedia(Media.MediaList mediaListToComplete)
        {
            var editDto = new MediaListEditDto
            {
                MediaId = mediaListToComplete.Media.Id,
                Progress = mediaListToComplete.Media.Episodes,
                Status = Media.MediaListStatus.Completed
            };

            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
                {
                    View.DisplaySnackbarMessage($"Completed {mediaList.Media.Title.UserPreferred}", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                })
                .Switch(error =>
                {
                    View.DisplaySnackbarMessage("Error occurred while saving list entry", Snackbar.LengthLong);
                    View.ResetMediaListItem(mediaListToComplete.Media.Id);
                });
        }

        public BaseRecyclerAdapter.RecyclerCardType GetCardType()
        {
            return AniDroidSettings.CardType;
        }

        public MediaListRecyclerAdapter.MediaListItemViewType GetMediaListItemViewType()
        {
            return AniDroidSettings.MediaViewType;
        }
    }
}