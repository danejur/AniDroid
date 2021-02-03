using System;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;

namespace AniDroid.AniListObject.Media
{
    public interface IAniListMediaListEditPresenter
    {
        Task SaveMediaListEntry(MediaListEditDto editDto, Action onSuccess, Action onError);
        Task DeleteMediaListEntry(int mediaListId, Action onSuccess, Action onError);
    }
}