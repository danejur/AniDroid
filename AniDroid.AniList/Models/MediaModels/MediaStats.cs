using System.Collections.Generic;
using System.Linq;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaStats
    {
        public List<AniListScoreDistribution> ScoreDistribution { get; set; }
        public List<AniListStatusDistribution> StatusDistribution { get; set; }
        public List<MediaAiringProgression> AiringProgression { get; set; }

        public bool AreStatsValid()
        {
            return ScoreDistribution?.Count(x => x.Amount > 0) >= 3 || AiringProgression?.Count >= 3 ||
                   StatusDistribution?.Any(x => x.Amount >= 3) == true;
        }
    }
}