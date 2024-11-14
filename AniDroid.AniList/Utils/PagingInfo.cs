namespace AniDroid.AniList.Utils
{
    public sealed class PagingInfo
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
        public bool? Remaining { get; set; }

        public PagingInfo(int pageSize)
        {
            PageSize = pageSize;
        }
    }
}
