namespace AniDroid.AniList.Dto
{
    public class FavoriteDto
    {
        public int? AnimeId { get; set; }
        public int? MangaId { get; set; }
        public int? CharacterId { get; set; }
        public int? StaffId { get; set; }
        public int? StudioId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool HasValue =>
            AnimeId.HasValue || MangaId.HasValue || CharacterId.HasValue || StaffId.HasValue || StudioId.HasValue;

        [Newtonsoft.Json.JsonIgnore]
        public bool IsValid =>
            AnimeId.HasValue ^ MangaId.HasValue ^ CharacterId.HasValue ^ StaffId.HasValue ^ StudioId.HasValue;
    }
}
