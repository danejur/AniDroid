using System.Linq;
using AniDroid.AniList.Models;

namespace AniDroid.Adapters.ViewModels
{
    public class RecommendationEdgeViewModel : AniDroidAdapterViewModel<Recommendation.Edge>
    {
        private RecommendationEdgeViewModel(Recommendation.Edge model, RecommendationDetailType primaryRecommendationDetailType, RecommendationDetailType secondaryRecommendationDetailType) : base(model)
        {
            TitleText = Model.Node.MediaRecommendation.Title?.UserPreferred;
            DetailPrimaryText = GetDetail(primaryRecommendationDetailType);
            DetailSecondaryText = GetDetail(secondaryRecommendationDetailType);
            ImageUri = Model.Node.MediaRecommendation.CoverImage?.Large ?? Model.Node.MediaRecommendation.CoverImage?.Medium;
        }

        public enum RecommendationDetailType
        {
            None,
            Genres,
            Rating
        }

        public static RecommendationEdgeViewModel CreateRecommendationViewModel(Recommendation.Edge model)
        {
            return new RecommendationEdgeViewModel(model, RecommendationDetailType.Genres, RecommendationDetailType.Rating);
        }

        private string GetDetail(RecommendationDetailType recommendationDetailType)
        {
            var retString = recommendationDetailType switch
            {
                RecommendationDetailType.Genres => (Model.Node.MediaRecommendation.Genres?.Any() == true
                    ? string.Join(", ", Model.Node.MediaRecommendation.Genres)
                    : "(No Genres)"),
                RecommendationDetailType.Rating => $"Rating: {Model.Node.Rating:+#;-#;0}",
                _ => null
            };

            return retString;
        }
    }
}