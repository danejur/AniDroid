using System.Collections.Generic;

namespace AniDroid.AniList.Models
{
    public class AniListName
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Native { get; set; }
        public string Full { get; set; }
        public List<string> Alternative { get; set; }

        public string FormattedName => $"{First} {Last}".Trim();

        // TODO: remove this
        public string GetFormattedName(bool nativeLineBreak = false) =>
            $"{$"{First} {Last}".Trim()}{(string.IsNullOrWhiteSpace(Native) ? "" : ($"{(nativeLineBreak ? "\n" : " ")}({Native})"))}";
    }
}
