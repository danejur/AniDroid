﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Comparers;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Google.Android.Material.Snackbar;

namespace AniDroid.MediaList
{
    public class MediaListPresenter : BaseAniDroidPresenter<IMediaListView>, IAniListMediaListEditPresenter
    {
        public MediaListPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public async Task GetMediaLists(int userId)
        {
            var mediaListResp = await AniListService.GetUserMediaList(userId,
                View.GetMediaType(), AniDroidSettings.GroupCompletedLists, default);


            mediaListResp.Switch(error => View.OnError(error))
                .Switch(mediaLists =>
                {
                    if (userId == AniDroidSettings.LoggedInUser?.Id)
                    {
                        AniDroidSettings.UpdateLoggedInUser(mediaLists.User);
                    }

                    View.SetCollection(mediaLists);
                });
        }

        public async Task SaveMediaListEntry(MediaListEditDto editDto, Action onSuccess, Action onError)
        {
            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default);

            mediaUpdateResp.Switch(mediaList =>
                {
                    onSuccess();
                    View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                }).Switch(error => onError());
        }

        public async Task DeleteMediaListEntry(int mediaListId, Action onSuccess, Action onError)
        {
            var mediaDeleteResp = await AniListService.DeleteMediaListEntry(mediaListId, default);

            mediaDeleteResp.Switch((bool success) =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Deleted", Snackbar.LengthShort);
                View.RemoveMediaListItem(mediaListId);
            }).Switch(error => 
                onError());
        }

        public async Task IncreaseMediaProgress(AniList.Models.MediaModels.MediaList mediaListToUpdate)
        {
            var editDto = new MediaListEditDto
            {
                MediaId = mediaListToUpdate.Media.Id,
                Progress = (mediaListToUpdate.Progress ?? 0) + 1
            };

            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default);

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

        public async Task CompleteMedia(AniList.Models.MediaModels.MediaList mediaListToComplete)
        {
            var editDto = new MediaListEditDto
            {
                MediaId = mediaListToComplete.Media.Id,
                Progress = mediaListToComplete.Media.Episodes,
                Status = MediaListStatus.Completed
            };

            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default);

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

        public bool GetHighlightPriorityItems()
        {
            return AniDroidSettings.HighlightPriorityMediaListItems;
        }

        public MediaListRecyclerAdapter.MediaListProgressDisplayType GetProgressDisplayType()
        {
            return AniDroidSettings.MediaListProgressDisplay;
        }

        public bool GetUseLongClickForEpisodeAdd()
        {
            return AniDroidSettings.UseLongClickForEpisodeAdd;
        }

        public bool GetDisplayTimeUntilAiringAsCountdown()
        {
            return AniDroidSettings.DisplayUpcomingEpisodeTimeAsCountdown;
        }

        public bool GetUseSwipeToRefreshOnMediaLists()
        {
            return AniDroidSettings.UseSwipeToRefreshOnMediaLists;
        }

        public bool GetShowEpisodeAddButtonForRepeatingMedia()
        {
            return AniDroidSettings.ShowEpisodeAddButtonForRepeatingMedia;
        }

        public MediaListSortComparer.SortDirection GetMediaListSortDirection(MediaType mediaType)
        {
            if (MediaType.Anime.Equals(mediaType))
            {
                return AniDroidSettings.AnimeListSortDirection;
            }
            else if (MediaType.Manga.Equals(mediaType))
            {
                return AniDroidSettings.MangaListSortDirection;
            }

            return MediaListSortComparer.SortDirection.Ascending;
        }

        public MediaListSortComparer.MediaListSortType GetMediaListSortType(MediaType mediaType)
        {
            if (MediaType.Anime.Equals(mediaType))
            {
                return AniDroidSettings.AnimeListSortType;
            }

            if (MediaType.Manga.Equals(mediaType))
            {
                return AniDroidSettings.MangaListSortType;
            }

            return MediaListSortComparer.MediaListSortType.NoSort;
        }

        public void SetMediaListSortSettings(MediaType mediaType, MediaListSortComparer.MediaListSortType sort,
            MediaListSortComparer.SortDirection direction)
        {
            if (MediaType.Anime.Equals(mediaType))
            {
                AniDroidSettings.AnimeListSortType = sort;
                AniDroidSettings.AnimeListSortDirection = direction;
            }
            else if (MediaType.Manga.Equals(mediaType))
            {
                AniDroidSettings.MangaListSortType = sort;
                AniDroidSettings.MangaListSortDirection = direction;
            }
        }

        public IList<MediaTag> GetMediaTags()
        {
            return AniDroidSettings.MediaTagCache;
        }

        public IList<string> GetGenres()
        {
            return AniDroidSettings.GenreCache;
        }
    }
}