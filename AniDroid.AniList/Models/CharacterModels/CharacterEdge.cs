using System.Collections.Generic;
using AniDroid.AniList.Enums.CharacterEnums;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.StaffModels;

namespace AniDroid.AniList.Models.CharacterModels
{
    public class CharacterEdge : ConnectionEdge<Character>
    {
        public CharacterRole Role { get; set; }
        public List<Staff> VoiceActors { get; set; }
        public List<Media> Media { get; set; }
        public int FavouriteOrder { get; set; }
    }
}