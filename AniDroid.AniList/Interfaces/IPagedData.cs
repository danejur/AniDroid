using System.Collections.Generic;
using AniDroid.AniList.DataTypes;

namespace AniDroid.AniList.Interfaces
{
    public interface IPagedData<T>
    {
        PageInfo PageInfo { get; set; }
        ICollection<T> Data { get; set; }
    }
}
