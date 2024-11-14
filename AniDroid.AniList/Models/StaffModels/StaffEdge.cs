namespace AniDroid.AniList.Models.StaffModels
{
    public class StaffEdge : ConnectionEdge<Staff>
    {
        public string Role { get; set; }
        public int FavouriteOrder { get; set; }
    }
}