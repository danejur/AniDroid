using System.Collections.Generic;

namespace AniDroid.AniList.Models.UserModels
{
    public class MediaListTypeOptions
    {
        public List<string> SectionOrder { get; set; }
        public bool SplitCompletedSectionByFormat { get; set; }
        public List<string> CustomLists { get; set; }
        public List<string> AdvancedScoring { get; set; }
        public bool AdvancedScoringEnabled { get; set; }
    }
}