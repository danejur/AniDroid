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

namespace AniDroid.AniListObject.Media
{
    public interface IAniListMediaListEditPresenter
    {
        Task SaveMediaListEntry(MediaListEditDto editDto);
    }
}