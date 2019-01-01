using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Models;

namespace AniDroid.Adapters.ViewModels
{
    public class ReviewViewModel : AniDroidAdapterViewModel<Review>
    {
        public Color ImageColor { get; protected set; }

        public ReviewViewModel(Review model, ReviewDetailType primaryReviewDetailType, ReviewDetailType secondaryReviewDetailType) : base(model)
        {
            TitleText = GetDetail(primaryReviewDetailType);
            DetailPrimaryText = GetDetail(secondaryReviewDetailType);
        }

        public enum ReviewDetailType
        {
            None,
            Summary,
            MediaTitle,
            Rating,
            Score,
            RatingAndScore
        }

        public static ReviewViewModel CreateMediaReviewViewModel(Review model)
        {
            return new ReviewViewModel(model, ReviewDetailType.Summary, ReviewDetailType.Rating)
            {
                ImageUri = model.User?.Avatar?.Large ?? model.User?.Avatar?.Medium
            };
        }

        public static ReviewViewModel CreateUserReviewViewModel(Review model)
        {
            return new ReviewViewModel(model, ReviewDetailType.MediaTitle, ReviewDetailType.Rating)
            {
                ImageUri = model.Media?.CoverImage?.Large ?? model.Media?.CoverImage?.Medium
            };
        }

        private string GetDetail(ReviewDetailType detailType)
        {
            string retString = null;

            if (detailType == ReviewDetailType.Summary)
            {
                retString = Model.Summary;
            }
            else if (detailType == ReviewDetailType.MediaTitle)
            {
                retString = Model.Media?.Title?.UserPreferred;
            }
            else if (detailType == ReviewDetailType.Rating)
            {
                retString = $"Rating:  {Model.Rating} / {Model.RatingAmount}";
            }

            return retString;
        }
    }
}