using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.CurrentSeason
{
    public interface ICurrentSeasonView : IAniDroidView
    {
        void ShowCurrentTv(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowCurrentMovies(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowCurrentOvaOna(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
        void ShowCurrentLeftovers(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable);
    }
}