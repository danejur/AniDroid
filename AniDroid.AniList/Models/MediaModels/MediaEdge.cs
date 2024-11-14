using System.Collections.Generic;
using AniDroid.AniList.Enums.CharacterEnums;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.StaffModels;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaEdge : ConnectionEdge<Media>
    {
        public MediaRelation RelationType { get; set; }
        public bool IsMainStudio { get; set; }
        public List<Character> Characters { get; set; }
        public CharacterRole CharacterRole { get; set; }
        public string StaffRole { get; set; }
        public List<Staff> VoiceActors { get; set; }
        public int FavouriteOrder { get; set; }
    }
}