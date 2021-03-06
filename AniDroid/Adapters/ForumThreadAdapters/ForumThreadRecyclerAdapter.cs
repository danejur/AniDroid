﻿using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ForumModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.ForumThreadAdapters
{
    public class ForumThreadRecyclerAdapter : AniDroidRecyclerAdapter<ForumThreadViewModel, ForumThread>
    {
        public ForumThreadRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> enumerable,
            Func<ForumThread, ForumThreadViewModel> createViewModelFunc) : base(context, enumerable,
            RecyclerCardType.Horizontal, createViewModelFunc)
        {
            ClickAction = (viewModel, position) =>
            {
                var intent = new Intent(Intent.ActionView);
                intent.SetData(Android.Net.Uri.Parse(viewModel.Model.SiteUrl));
                Context.StartActivity(intent);
            };
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }
    }
}