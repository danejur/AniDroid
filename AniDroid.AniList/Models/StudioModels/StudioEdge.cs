namespace AniDroid.AniList.Models.StudioModels
{
    public class StudioEdge : ConnectionEdge<Studio>
    {
        public bool IsMain { get; set; }
        public int FavouriteOrder { get; set; }
    }
}