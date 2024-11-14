using System.Collections.Generic;
using AniDroid.AniList.Interfaces;

namespace AniDroid.AniList.DataTypes
{
    public class PagedData<T> : IPagedData<T>
    {
        public PageInfo PageInfo { get; set; }
        public ICollection<T> Data { get; set; }
    }
}
