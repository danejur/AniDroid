using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.RecommendationModels;

namespace AniDroid.Adapters.ViewModels
{
    public class RecommendationEdgeViewModel : AniDroidAdapterViewModel<ConnectionEdge<Recommendation>>
    {
        private RecommendationEdgeViewModel(ConnectionEdge<Recommendation> model, RecommendationDetailType primaryRecommendationDetailType, RecommendationDetailType secondaryRecommendationDetailType) : base(model)
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

        public static RecommendationEdgeViewModel CreateRecommendationViewModel(ConnectionEdge<Recommendation> model)
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