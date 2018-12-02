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

namespace AniDroid.Adapters.ViewModels
{
    public class CharacterViewModel : AniDroidAdapterViewModel<Character>
    {
        public Character.Edge ModelEdge { get; protected set; }

        private CharacterViewModel(Character model, CharacterDetailType primaryCharacterDetailType, CharacterDetailType secondaryCharacterDetailType) : base(model)
        {
            TitleText = $"{Model.Name?.FormattedName}";
            DetailPrimaryText = GetDetail(primaryCharacterDetailType);
            DetailSecondaryText = GetDetail(secondaryCharacterDetailType);
            ImageUri = model.Image?.Large ?? model.Image?.Medium;
        }

        public enum CharacterDetailType
        {
            None,
            NativeName,
            Role
        }

        public static CharacterViewModel CreateCharacterViewModel(Character model)
        {
            return new CharacterViewModel(model, CharacterDetailType.NativeName, CharacterDetailType.None);
        }

        public static CharacterViewModel CreateMediaCharacterViewModel(Character model, Character.Edge edge)
        {
            return new CharacterViewModel(model, CharacterDetailType.NativeName, CharacterDetailType.None)
            {
                ModelEdge = edge
            };
        }

        private string GetDetail(CharacterDetailType detailType)
        {
            var retString = "";

            if (detailType == CharacterDetailType.NativeName)
            {
                retString = $"{Model.Name?.Native}";
            }
            else if (detailType == CharacterDetailType.Role)
            {
                retString = $"{ModelEdge?.Role?.DisplayValue}";
            }

            return retString;
        }
    }
}