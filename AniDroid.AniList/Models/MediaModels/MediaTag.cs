namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Rank { get; set; }
        public bool IsGeneralSpoiler { get; set; }
        public bool IsMediaSpoiler { get; set; }
        public bool IsAdult { get; set; }
    }
}