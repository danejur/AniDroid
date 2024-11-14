using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;

namespace AniDroid.AniList.Models.CharacterModels
{
    public class Character : AniListObject
    {
        public AniListName Name { get; set; }
        public AniListImage Image { get; set; }
        public string Description { get; set; }
        public bool IsFavourite { get; set; }
        public string SiteUrl { get; set; }
        public Connection<MediaEdge, Media> Media { get; set; }
        public IPagedData<MediaEdge> Anime { get; set; }
        public IPagedData<MediaEdge> Manga { get; set; }
    }
}
