using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;

namespace AniDroid.AniList.Models.UserModels
{
    public class UserFavourites
    {
        public Connection<MediaEdge, Media> Anime { get; set; }
        public Connection<MediaEdge, Media> Manga { get; set; }
        public Connection<CharacterEdge, Character> Characters { get; set; }
        public Connection<StaffEdge, Staff> Staff { get; set; }
        public Connection<StudioEdge, Studio> Studios { get; set; }
    }
}
