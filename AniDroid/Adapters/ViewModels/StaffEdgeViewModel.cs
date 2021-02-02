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
using AniDroid.AniList.Models.StaffModels;

namespace AniDroid.Adapters.ViewModels
{
    public class StaffEdgeViewModel : AniDroidAdapterViewModel<StaffEdge>
    {
        public StaffEdgeViewModel(StaffEdge model, StaffEdgeDetailType primaryStaffEdgeDetailType,
            StaffEdgeDetailType secondaryStaffEdgeDetailType) : base(model)
        {
            TitleText = $"{Model.Node?.Name?.Full ?? Model.Node?.Name?.FormattedName}";
            DetailPrimaryText = GetDetail(primaryStaffEdgeDetailType);
            DetailSecondaryText = GetDetail(secondaryStaffEdgeDetailType);
            ImageUri = Model.Node?.Image?.Large ?? Model?.Node?.Image?.Medium;
        }

        public enum StaffEdgeDetailType
        {
            None,
            NativeName,
            Role
        }

        public static StaffEdgeViewModel CreateStaffEdgeViewModel(StaffEdge model)
        {
            return new StaffEdgeViewModel(model, StaffEdgeDetailType.NativeName, StaffEdgeDetailType.Role);
        }

        public static StaffEdgeViewModel CreateFavoriteStaffEdgeViewModel(StaffEdge model)
        {
            return new StaffEdgeViewModel(model, StaffEdgeDetailType.NativeName, StaffEdgeDetailType.None);
        }

        private string GetDetail(StaffEdgeDetailType detailType)
        {
            string retString = null;

            if (detailType == StaffEdgeDetailType.NativeName)
            {
                retString = $"{Model.Node?.Name?.Native}";
            }
            else if (detailType == StaffEdgeDetailType.Role)
            {
                retString = $"{Model?.Role}";
            }

            return retString;
        }
    }
}