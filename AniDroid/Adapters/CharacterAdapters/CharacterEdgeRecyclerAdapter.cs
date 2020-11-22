using System;
using System.Collections.Generic;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Character;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Adapters.CharacterAdapters
{
    public class CharacterEdgeRecyclerAdapter : AniDroidRecyclerAdapter<CharacterEdgeViewModel, Character.Edge>
    {
        public int ButtonIconResourceId { get; set; }

        public CharacterEdgeRecyclerAdapter(BaseAniDroidActivity context,
            IAsyncEnumerable<OneOf<IPagedData<Character.Edge>, IAniListError>> enumerable, RecyclerCardType cardType,
            Func<Character.Edge, CharacterEdgeViewModel> createViewModelFunc) : base(context, enumerable, cardType,
            createViewModelFunc)
        {
            ClickAction = (viewModel, position) =>
                CharacterActivity.StartActivity(Context, viewModel.Model.Node.Id,
                    BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}