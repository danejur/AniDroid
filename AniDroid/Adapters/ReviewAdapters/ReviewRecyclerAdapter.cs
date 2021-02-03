using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ReviewModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.ReviewAdapters
{
    public class ReviewRecyclerAdapter : AniDroidRecyclerAdapter<ReviewViewModel, Review>
    {
        public ReviewRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<Review, ReviewViewModel> createViewModelFunc) : base(context, enumerable, cardType,
            createViewModelFunc)
        {
            ClickAction = (viewModel, position) =>
            {
                Toast.MakeText(Application.Context, "In-app review viewing coming Soon™", ToastLength.Short).Show();
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://anilist.co/review/{viewModel.Model.Id}"));
                Context.StartActivity(intent);
            };
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            item.Name.SetSingleLine(false);
            item.Name.SetMaxLines(2);
            return item;
        }
    }
}