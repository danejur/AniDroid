using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Task = System.Threading.Tasks.Task;

namespace AniDroid.Discover
{
    public class DiscoverPresenter : BaseAniDroidPresenter<IDiscoverView>
    {
        public DiscoverPresenter(IDiscoverView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void GetDiscoverLists()
        {
            View.ShowCurrentlyAiringResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Season = Media.MediaSeason.Fall,
                SeasonYear = 2018,
                Type = Media.MediaType.Anime,
                Sort = new List<Media.MediaSort> { Media.MediaSort.PopularityDesc }
            }, 5));
            View.ShowTrendingResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Sort = new List<Media.MediaSort> {Media.MediaSort.TrendingDesc}
            }, 5));
            View.ShowNewAnimeResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = Media.MediaType.Anime,
                    Sort = new List<Media.MediaSort> { Media.MediaSort.IdDesc }
                }, 5));
            View.ShowNewMangaResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = Media.MediaType.Manga,
                    Sort = new List<Media.MediaSort> { Media.MediaSort.IdDesc }
                }, 5));
        }
    }
}