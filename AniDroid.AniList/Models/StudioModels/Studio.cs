using AniDroid.AniList.Models.MediaModels;

namespace AniDroid.AniList.Models.StudioModels
{
    public class Studio : AniListObject
    {
        public string Name { get; set; }
        public Connection<MediaEdge, Media> Media { get; set; }
        public string SiteUrl { get; set; }
        public bool IsFavourite { get; set; }
    }
}
