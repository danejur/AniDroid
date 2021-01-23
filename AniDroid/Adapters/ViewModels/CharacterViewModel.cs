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
using AniDroid.AniList.Models.CharacterModels;

namespace AniDroid.Adapters.ViewModels
{
    public class CharacterViewModel : AniDroidAdapterViewModel<Character>
    {
        private CharacterViewModel(Character model, CharacterDetailType primaryCharacterDetailType, CharacterDetailType secondaryCharacterDetailType, bool isButtonVisible) : base(model)
        {
            TitleText = Model.Name?.FormattedName;
            DetailPrimaryText = GetDetail(primaryCharacterDetailType);
            DetailSecondaryText = GetDetail(secondaryCharacterDetailType);
            ImageUri = model.Image?.Large ?? model.Image?.Medium;
            IsButtonVisible = isButtonVisible;
        }

        public enum CharacterDetailType
        {
            None,
            NativeName
        }

        public static CharacterViewModel CreateCharacterViewModel(Character model)
        {
            return new CharacterViewModel(model, CharacterDetailType.NativeName, CharacterDetailType.None, model.IsFavourite);
        }

        private string GetDetail(CharacterDetailType detailType)
        {
            string retString = null;

            if (detailType == CharacterDetailType.NativeName)
            {
                retString = $"{Model.Name?.Native}";
            }
            //else if (detailType == CharacterDetailType.Role)
            //{
            //    retString = $"{ModelEdge?.Role?.DisplayValue}";
            //}

            return retString;
        }
    }
}