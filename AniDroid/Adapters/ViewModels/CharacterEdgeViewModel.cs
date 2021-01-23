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
    public class CharacterEdgeViewModel : AniDroidAdapterViewModel<CharacterEdge>
    {
        public CharacterEdgeViewModel(CharacterEdge model, CharacterEdgeDetailType primaryCharacterEdgeDetailType,
            CharacterEdgeDetailType secondaryCharacterEdgeDetailType, bool isButtonVisible, int? buttonIcon) : base(model)
        {
            TitleText = $"{Model.Node?.Name?.FormattedName}";
            DetailPrimaryText = GetDetail(primaryCharacterEdgeDetailType);
            DetailSecondaryText = GetDetail(secondaryCharacterEdgeDetailType);
            ImageUri = Model.Node?.Image?.Large ?? Model?.Node?.Image?.Medium;
            IsButtonVisible = isButtonVisible;
            ButtonIcon = buttonIcon;
        }

        public enum CharacterEdgeDetailType
        {
            None,
            NativeName,
            Role
        }

        public static CharacterEdgeViewModel CreateMediaCharacterEdgeViewModel(CharacterEdge model, int? buttonIcon = null)
        {
            return new CharacterEdgeViewModel(model, CharacterEdgeDetailType.NativeName, CharacterEdgeDetailType.Role,
                model.VoiceActors?.Any() == true, buttonIcon);
        }

        public static CharacterEdgeViewModel CreateStaffCharacterEdgeViewModel(CharacterEdge model)
        {
            return new CharacterEdgeViewModel(model, CharacterEdgeDetailType.NativeName, CharacterEdgeDetailType.Role,
                false, null);
        }
        
        private string GetDetail(CharacterEdgeDetailType detailType)
        {
            string retString = null;

            if (detailType == CharacterEdgeDetailType.NativeName)
            {
                retString = $"{Model.Node?.Name?.Native}";
            }
            else if (detailType == CharacterEdgeDetailType.Role)
            {
                retString = $"{Model?.Role?.DisplayValue}";
            }

            return retString;
        }
    }
}